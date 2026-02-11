window.simpleVideo = {
    localStream: null,
    screenStream: null,
    peer: null,
    activeCalls: {},
    screenCalls: {},
    remoteStreams: {}, // Cache for stream reloading
    dotNetRef: null,

    init: async (dotNetRef) => {
        console.log("[JS] Initializing PeerJS...");
        window.simpleVideo.dotNetRef = dotNetRef;

        const peer = new Peer(null, {
            debug: 2,
            config: {
                iceServers: [
                    { urls: 'stun:stun.l.google.com:19302' }
                ]
            }
        });
        window.simpleVideo.peer = peer;

        return new Promise((resolve, reject) => {
            peer.on('open', (id) => {
                console.log(`[JS] Peer Opened. My ID: ${id}`);

                peer.on('call', (call) => {
                    console.log(`[JS] Incoming call from: ${call.peer}`);

                    // Answer logic
                    if (call.metadata && call.metadata.type === 'screen') {
                        call.answer();
                    } else {
                        // Answer with local stream (or undefined if spectator)
                        call.answer(window.simpleVideo.localStream || undefined);
                    }
                    window.simpleVideo.registerCallEvents(call, dotNetRef);
                });
                resolve(id);
            });

            peer.on('error', (err) => console.error('[JS] Peer Error:', err));
        });
    },

    registerCallEvents: (call, dotNetRef) => {
        const isScreen = call.metadata && call.metadata.type === 'screen';
        console.log(`[JS] Registering events for call with: ${call.peer} (Screen: ${isScreen})`);

        call.on('stream', (remoteStream) => {
            console.log(`[JS] Stream received from ${call.peer}`);

            // 1. Cache the stream so Blazor can reload it later
            window.simpleVideo.remoteStreams[call.peer] = remoteStream;

            // 2. Notify Blazor to update UI
            if (dotNetRef) {
                const method = isScreen ? 'OnScreenShareAdded' : 'OnRemoteStreamAdded';
                const args = isScreen ? [call.peer, remoteStream.id] : [call.peer];
                dotNetRef.invokeMethodAsync(method, ...args);
            }

            // 3. Attach immediately (in case DOM is ready)
            window.simpleVideo.retryAttachStream(call.peer, remoteStream, isScreen);
        });

        if (isScreen) window.simpleVideo.screenCalls[call.peer] = call;
        else window.simpleVideo.activeCalls[call.peer] = call;

        call.on('close', () => console.log(`[JS] Call with ${call.peer} closed.`));
        call.on('error', (err) => console.error(`[JS] Call Error (${call.peer}):`, err));
    },

    retryAttachStream: (peerId, stream, isScreen) => {
        const elementId = isScreen ? `screen-${peerId}` : `video-${peerId}`;
        let attempts = 0;

        const tryAttach = () => {
            const el = document.getElementById(elementId);
            if (el) {
                // --- ROBUSTNESS CHECK ---
                // If element is already playing THIS content, ignore update to prevent flicker/reset
                if (el.srcObject) {
                    try {
                        const currentTracks = el.srcObject.getTracks().map(t => t.id).sort().join('');
                        const newTracks = stream.getTracks().map(t => t.id).sort().join('');
                        if (currentTracks === newTracks && !el.paused && !el.ended) {
                            return;
                        }
                    } catch (e) { /* ignore comparison errors */ }
                }

                console.log(`[JS] Attaching stream to #${elementId}`);

                el.autoplay = true;
                el.playsInline = true;
                el.muted = true; // Critical for autoplay policy
                el.srcObject = stream;

                const playPromise = el.play();
                if (playPromise !== undefined) {
                    playPromise
                        .then(() => {
                            // Unmute user video after it starts playing
                            if (!isScreen) el.muted = false;
                        })
                        .catch(e => console.warn(`[JS] Autoplay blocked for ${elementId}:`, e));
                }
            } else if (attempts < 50) {
                attempts++;
                setTimeout(tryAttach, 100);
            }
        };
        tryAttach();
    },

    // --- CALLED BY BLAZOR OnRemoteStreamAdded ---
    reloadVideo: (peerId) => {
        console.log(`[JS] Reloading video for ${peerId}...`);
        const stream = window.simpleVideo.remoteStreams ? window.simpleVideo.remoteStreams[peerId] : null;
        if (stream) {
            window.simpleVideo.retryAttachStream(peerId, stream, false);
        } else {
            console.warn(`[JS] No cached stream found for ${peerId} to reload.`);
        }
    },

    startLocalStream: async (initMuted, initVideoOff) => {
        try {
            const constraints = {
                audio: true,
                video: initVideoOff ? false : true
            };

            let stream;
            try {
                stream = await navigator.mediaDevices.getUserMedia(constraints);
            } catch (e) {
                console.warn("[JS] Video failed, trying audio only", e);
                stream = await navigator.mediaDevices.getUserMedia({ audio: true, video: false });
            }

            window.simpleVideo.localStream = stream;

            if (initMuted) {
                stream.getAudioTracks().forEach(t => t.enabled = false);
            }

            window.simpleVideo.attachLocalVideo();
            return true;
        } catch (err) {
            console.error("[JS] Camera access denied:", err);
            return false;
        }
    },

    attachLocalVideo: () => {
        const videoEl = document.getElementById("localVideo");
        if (videoEl && window.simpleVideo.localStream) {
            const vidTracks = window.simpleVideo.localStream.getVideoTracks();
            if (vidTracks.length > 0) {
                videoEl.srcObject = window.simpleVideo.localStream;
                videoEl.muted = true;
                videoEl.play().catch(e => console.warn("Local play error", e));
            } else {
                videoEl.srcObject = null;
            }
        }
    },

    // ... Keep attachLocalScreen, ensureVideoPlays, callUsers, shareScreenWithUser ...
    attachLocalScreen: (peerId) => {
        const videoEl = document.getElementById(`screen-${peerId}`);
        if (videoEl && window.simpleVideo.screenStream) {
            videoEl.srcObject = window.simpleVideo.screenStream;
            videoEl.muted = true;
        }
    },
    ensureVideoPlays: (peerId) => {
        const el = document.getElementById(`video-${peerId}`);
        if (el) el.play().catch(e => { });
    },
    callUsers: (peerIds) => {
        peerIds.forEach(peerId => {
            const call = window.simpleVideo.peer.call(peerId, window.simpleVideo.localStream);
            window.simpleVideo.registerCallEvents(call, window.simpleVideo.dotNetRef);
        });
    },
    shareScreenWithUser: (peerId) => {
        if (window.simpleVideo.screenStream && window.simpleVideo.peer) {
            const call = window.simpleVideo.peer.call(peerId, window.simpleVideo.screenStream, { metadata: { type: 'screen' } });
            window.simpleVideo.registerCallEvents(call, window.simpleVideo.dotNetRef);
        }
    },

    // --- FIX FOR MISSING FUNCTION ---
    toggleAudio: (shouldEnable) => {
        console.log(`[JS] Toggling Audio: ${shouldEnable}`);
        if (window.simpleVideo.localStream) {
            window.simpleVideo.localStream.getAudioTracks().forEach(t => t.enabled = shouldEnable);
        }
    },

    toggleVideo: async (shouldEnable) => {
        console.log(`[JS] Toggling Video: ${shouldEnable}`);
        let localStream = window.simpleVideo.localStream;

        if (!shouldEnable && !localStream) return;

        if (!shouldEnable) {
            if (localStream) {
                localStream.getVideoTracks().forEach(t => {
                    t.stop();
                    localStream.removeTrack(t);
                });
            }
            window.simpleVideo.attachLocalVideo();
        }
        else {
            try {
                const newStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
                const oldAudio = localStream ? localStream.getAudioTracks()[0] : null;
                const isMuted = oldAudio ? !oldAudio.enabled : false;

                newStream.getAudioTracks().forEach(t => t.enabled = !isMuted);
                if (localStream) localStream.getTracks().forEach(t => t.stop());

                window.simpleVideo.localStream = newStream;
                window.simpleVideo.attachLocalVideo();

                const newVideoTrack = newStream.getVideoTracks()[0];
                const newAudioTrack = newStream.getAudioTracks()[0];

                for (let peerId in window.simpleVideo.activeCalls) {
                    const call = window.simpleVideo.activeCalls[peerId];
                    // Skip dead connections
                    if (!call || !call.peerConnection || call.peerConnection.signalingState === 'closed') continue;

                    try {
                        const senders = call.peerConnection.getSenders();
                        const videoSender = senders.find(s => s.track && s.track.kind === 'video');
                        const audioSender = senders.find(s => s.track && s.track.kind === 'audio');

                        if (videoSender) {
                            await videoSender.replaceTrack(newVideoTrack);
                        } else {
                            call.close();
                            const newCall = window.simpleVideo.peer.call(peerId, newStream);
                            window.simpleVideo.registerCallEvents(newCall, window.simpleVideo.dotNetRef);
                        }
                        if (audioSender) {
                            await audioSender.replaceTrack(newAudioTrack);
                        }
                    } catch (e) { console.error(`[JS] Error updating peer ${peerId}`, e); }
                }
            } catch (err) { console.error("Error toggling video:", err); }
        }
    },

    startScreenShare: async () => {
        try {
            const stream = await navigator.mediaDevices.getDisplayMedia({ video: true, audio: false });
            window.simpleVideo.screenStream = stream;
            const peers = Object.keys(window.simpleVideo.activeCalls);
            peers.forEach(peerId => {
                const call = window.simpleVideo.peer.call(peerId, stream, { metadata: { type: 'screen' } });
                window.simpleVideo.registerCallEvents(call, window.simpleVideo.dotNetRef);
            });
            stream.getVideoTracks()[0].onended = () => { window.simpleVideo.stopScreenShare(); };
            return true;
        } catch (err) { return false; }
    },
    stopScreenShare: () => {
        if (window.simpleVideo.screenStream) {
            window.simpleVideo.screenStream.getTracks().forEach(t => t.stop());
            window.simpleVideo.screenStream = null;
        }
        for (let peerId in window.simpleVideo.screenCalls) {
            if (window.simpleVideo.screenCalls[peerId]) window.simpleVideo.screenCalls[peerId].close();
        }
        window.simpleVideo.screenCalls = {};
    },
    leave: () => {
        if (window.simpleVideo.localStream) {
            window.simpleVideo.localStream.getTracks().forEach(track => track.stop());
            window.simpleVideo.localStream = null;
        }
        window.simpleVideo.stopScreenShare();
        if (window.simpleVideo.peer) {
            window.simpleVideo.peer.destroy();
            window.simpleVideo.peer = null;
        }
        window.simpleVideo.activeCalls = {};
        window.simpleVideo.remoteStreams = {}; // Clear cache
    }
};