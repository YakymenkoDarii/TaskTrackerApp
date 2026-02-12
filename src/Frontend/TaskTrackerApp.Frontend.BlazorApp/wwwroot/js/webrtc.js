window.simpleVideo = {
    localStream: null,
    screenStream: null,
    peer: null,
    dotNetRef: null,
    selectedAudioId: null,
    selectedVideoId: null,
    calls: {},
    remoteStreams: {},
    screenStreams: {},

    createBlackTrack: () => {
        const canvas = document.createElement('canvas');
        canvas.width = 640;
        canvas.height = 480;
        const ctx = canvas.getContext('2d');
        setInterval(() => {
            ctx.fillStyle = 'black';
            ctx.fillRect(0, 0, 640, 480);
            const val = Math.floor(Math.random() * 255);
            ctx.fillStyle = `rgba(${val}, 0, 0, 0.01)`;
            ctx.fillRect(0, 0, 1, 1);
        }, 33);
        const stream = canvas.captureStream(30);
        const track = stream.getVideoTracks()[0];
        track.enabled = true;
        return track;
    },

    isRealCamera: (track) => {
        if (!track || track.readyState !== 'live') return false;
        return track.label !== '' && !track.label.includes('Canvas');
    },

    init: async (dotNetRef) => {
        window.simpleVideo.dotNetRef = dotNetRef;
        return new Promise((resolve, reject) => {
            const peer = new Peer(null);

            peer.on('open', (id) => {
                window.simpleVideo.peer = peer;
                resolve(id);
            });

            peer.on('call', (call) => {
                const metadata = call.metadata || {};
                if (metadata.type === 'screen') {
                    call.answer();
                    call.on('stream', (remoteScreenStream) => {
                        window.simpleVideo.screenStreams[call.peer] = remoteScreenStream;
                        dotNetRef.invokeMethodAsync('OnScreenShareAdded', call.peer, remoteScreenStream.id)
                            .then(() => window.simpleVideo.reloadVideo(call.peer, true, false));
                    });
                } else {
                    const streamToAnswer = new MediaStream();
                    const audio = window.simpleVideo.localStream.getAudioTracks()[0];
                    if (audio) streamToAnswer.addTrack(audio);

                    const video = window.simpleVideo.localStream.getVideoTracks().find(t => t.readyState === 'live' && t.enabled);
                    if (video && window.simpleVideo.isRealCamera(video)) streamToAnswer.addTrack(video);
                    else streamToAnswer.addTrack(window.simpleVideo.createBlackTrack());

                    call.answer(streamToAnswer);
                    window.simpleVideo.calls[call.peer] = call;
                    call.on('stream', (remoteStream) => {
                        window.simpleVideo.remoteStreams[call.peer] = remoteStream;
                        const checkVideoState = () => {
                            const validVideo = remoteStream.getVideoTracks().some(t => window.simpleVideo.isRealCamera(t));
                            window.simpleVideo.dotNetRef.invokeMethodAsync('OnRemoteStreamAdded', call.peer, validVideo);
                            if (validVideo) window.simpleVideo.reloadVideo(call.peer, false, false);
                        };
                        checkVideoState();
                        remoteStream.onaddtrack = checkVideoState;
                        remoteStream.onremovetrack = checkVideoState;
                    });
                }
            });
        });
    },

    reloadVideo: (peerId, isScreen = false, shouldMute = false) => {
        const elementId = isScreen ? `screen-${peerId}` : `video-${peerId}`;
        const stream = isScreen ? window.simpleVideo.screenStreams[peerId] : window.simpleVideo.remoteStreams[peerId];
        const attemptPlay = (attemptNumber) => {
            const videoElement = document.getElementById(elementId);
            if (videoElement && stream) {
                if (videoElement.srcObject !== stream) videoElement.srcObject = stream;
                videoElement.muted = shouldMute;
                videoElement.play().catch(e => { });
            } else if (attemptNumber < 10) {
                setTimeout(() => attemptPlay(attemptNumber + 1), 200 + (attemptNumber * 100));
            }
        };
        attemptPlay(1);
    },

    ensureVideoPlays: (peerId) => window.simpleVideo.reloadVideo(peerId, false, false),

    attachLocalScreen: (peerId) => {
        if (window.simpleVideo.screenStream) window.simpleVideo.screenStreams[peerId] = window.simpleVideo.screenStream;
        window.simpleVideo.reloadVideo(peerId, true, true);
    },

    removePeer: (peerId) => {
        if (window.simpleVideo.calls[peerId]) {
            window.simpleVideo.calls[peerId].close();
            delete window.simpleVideo.calls[peerId];
        }
        delete window.simpleVideo.remoteStreams[peerId];
        delete window.simpleVideo.screenStreams[peerId];

        const videoEl = document.getElementById(`video-${peerId}`);
        if (videoEl) videoEl.srcObject = null;
        const screenEl = document.getElementById(`screen-${peerId}`);
        if (screenEl) screenEl.srcObject = null;
    },

    getDevices: async () => {
        try {
            const devices = await navigator.mediaDevices.enumerateDevices();
            return devices.map(d => ({
                deviceId: d.deviceId,
                kind: d.kind,
                label: d.label ? d.label : (d.kind === 'audioinput' ? `Microphone ${d.deviceId.substr(0, 5)}...` : `Camera ${d.deviceId.substr(0, 5)}...`)
            }));
        } catch (err) {
            return [];
        }
    },

    switchAudioDevice: async (deviceId) => {
        try {
            window.simpleVideo.selectedAudioId = deviceId;
            const stream = await navigator.mediaDevices.getUserMedia({
                audio: { deviceId: { exact: deviceId } },
                video: false
            });
            const newTrack = stream.getAudioTracks()[0];
            const oldTrack = window.simpleVideo.localStream.getAudioTracks()[0];
            if (oldTrack) {
                newTrack.enabled = oldTrack.enabled;
                oldTrack.stop();
                window.simpleVideo.localStream.removeTrack(oldTrack);
            }
            window.simpleVideo.localStream.addTrack(newTrack);

            for (const peerId in window.simpleVideo.calls) {
                const call = window.simpleVideo.calls[peerId];
                if (call && call.peerConnection) {
                    const sender = call.peerConnection.getSenders().find(s => s.track && s.track.kind === 'audio');
                    if (sender) await sender.replaceTrack(newTrack);
                }
            }
            return true;
        } catch (e) {
            return false;
        }
    },

    switchVideoDevice: async (deviceId) => {
        try {
            window.simpleVideo.selectedVideoId = deviceId;
            const currentVideo = window.simpleVideo.localStream.getVideoTracks()[0];
            const isBlackTrack = !window.simpleVideo.isRealCamera(currentVideo);

            if (isBlackTrack) return true;

            const stream = await navigator.mediaDevices.getUserMedia({
                video: { deviceId: { exact: deviceId } },
                audio: false
            });
            const newTrack = stream.getVideoTracks()[0];

            currentVideo.stop();
            window.simpleVideo.localStream.removeTrack(currentVideo);
            window.simpleVideo.localStream.addTrack(newTrack);

            const localVideo = document.getElementById('localVideo');
            if (localVideo) localVideo.srcObject = window.simpleVideo.localStream;

            for (const peerId in window.simpleVideo.calls) {
                const call = window.simpleVideo.calls[peerId];
                if (call && call.peerConnection) {
                    const sender = call.peerConnection.getSenders().find(s => s.track && s.track.kind === 'video');
                    if (sender) await sender.replaceTrack(newTrack);
                }
            }
            return true;
        } catch (e) {
            return false;
        }
    },

    startLocalStream: async (isMuted, isVideoOff) => {
        try {
            const constraints = {
                audio: window.simpleVideo.selectedAudioId ? { deviceId: { exact: window.simpleVideo.selectedAudioId } } : true,
                video: window.simpleVideo.selectedVideoId ? { deviceId: { exact: window.simpleVideo.selectedVideoId } } : true
            };
            const stream = await navigator.mediaDevices.getUserMedia(constraints);
            window.simpleVideo.localStream = stream;
            window.simpleVideo.toggleAudio(!isMuted);
            if (isVideoOff) await window.simpleVideo.toggleVideo(false);
            return true;
        } catch (err) { return false; }
    },

    attachLocalVideo: () => {
        const video = document.getElementById('localVideo');
        if (video && window.simpleVideo.localStream) {
            video.srcObject = window.simpleVideo.localStream;
            video.muted = true;
        }
    },

    callUsers: (peerIds) => {
        if (!window.simpleVideo.peer || !window.simpleVideo.localStream) return;
        const streamToSend = new MediaStream();

        const audio = window.simpleVideo.localStream.getAudioTracks()[0];
        if (audio) streamToSend.addTrack(audio);

        const video = window.simpleVideo.localStream.getVideoTracks().find(t => t.readyState === 'live' && t.enabled);
        if (video && window.simpleVideo.isRealCamera(video)) streamToSend.addTrack(video);
        else streamToSend.addTrack(window.simpleVideo.createBlackTrack());

        peerIds.forEach(peerId => {
            if (window.simpleVideo.calls[peerId]) return;
            const call = window.simpleVideo.peer.call(peerId, streamToSend, { metadata: { type: 'video' } });
            window.simpleVideo.calls[peerId] = call;
            call.on('stream', (remoteStream) => {
                window.simpleVideo.remoteStreams[peerId] = remoteStream;
                const checkVideoState = () => {
                    const validVideo = remoteStream.getVideoTracks().some(t => window.simpleVideo.isRealCamera(t));
                    window.simpleVideo.dotNetRef.invokeMethodAsync('OnRemoteStreamAdded', peerId, validVideo);
                    if (validVideo) window.simpleVideo.reloadVideo(peerId, false, false);
                };
                checkVideoState();
                remoteStream.onaddtrack = checkVideoState;
                remoteStream.onremovetrack = checkVideoState;
            });
        });
    },

    toggleAudio: (enabled) => {
        if (window.simpleVideo.localStream) window.simpleVideo.localStream.getAudioTracks().forEach(t => t.enabled = enabled);
    },

    toggleVideo: async (enabled) => {
        const localStream = window.simpleVideo.localStream;
        if (!localStream) return;
        try {
            let newTrack;
            if (enabled) {
                const constraints = window.simpleVideo.selectedVideoId
                    ? { video: { deviceId: { exact: window.simpleVideo.selectedVideoId } } }
                    : { video: true };
                const stream = await navigator.mediaDevices.getUserMedia(constraints);
                newTrack = stream.getVideoTracks()[0];
            } else {
                newTrack = window.simpleVideo.createBlackTrack();
            }
            localStream.getVideoTracks().forEach(t => t.stop());
            localStream.removeTrack(localStream.getVideoTracks()[0]);
            localStream.addTrack(newTrack);

            const localVideo = document.getElementById('localVideo');
            if (localVideo) localVideo.srcObject = localStream;

            for (const peerId in window.simpleVideo.calls) {
                const call = window.simpleVideo.calls[peerId];
                if (call && call.peerConnection) {
                    const sender = call.peerConnection.getSenders().find(s => s.track && s.track.kind === 'video');
                    if (sender) await sender.replaceTrack(newTrack);
                }
            }
        } catch (err) { }
    },

    startScreenShare: async () => {
        try {
            const stream = await navigator.mediaDevices.getDisplayMedia({ video: true, audio: true });
            window.simpleVideo.screenStream = stream;
            stream.getVideoTracks()[0].onended = () => {
                window.simpleVideo.stopScreenShare();
                window.simpleVideo.dotNetRef.invokeMethodAsync('ToggleScreenShare');
            };
            return true;
        } catch (err) { return false; }
    },

    stopScreenShare: () => {
        if (window.simpleVideo.screenStream) {
            window.simpleVideo.screenStream.getTracks().forEach(track => track.stop());
            window.simpleVideo.screenStream = null;
        }
    },

    shareScreenWithUser: (peerId) => {
        if (!window.simpleVideo.peer || !window.simpleVideo.screenStream) return;
        window.simpleVideo.peer.call(peerId, window.simpleVideo.screenStream, { metadata: { type: 'screen' } });
    },

    leave: () => {
        Object.values(window.simpleVideo.calls).forEach(call => call.close());
        window.simpleVideo.calls = {};
        window.simpleVideo.remoteStreams = {};
        window.simpleVideo.screenStreams = {};
        if (window.simpleVideo.localStream) {
            window.simpleVideo.localStream.getTracks().forEach(track => track.stop());
            window.simpleVideo.localStream = null;
        }
        window.simpleVideo.stopScreenShare();
        if (window.simpleVideo.peer) {
            window.simpleVideo.peer.destroy();
            window.simpleVideo.peer = null;
        }
    }
};