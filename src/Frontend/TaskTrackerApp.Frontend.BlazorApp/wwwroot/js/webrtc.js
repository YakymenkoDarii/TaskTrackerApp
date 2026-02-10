window.simpleVideo = {
    localStream: null,
    peer: null,
    activeCalls: {},

    init: async (dotNetRef) => {
        const peer = new Peer(null, { debug: 2 });
        window.simpleVideo.peer = peer;

        return new Promise((resolve, reject) => {
            peer.on('open', (id) => {
                peer.on('call', (call) => {
                    call.answer(window.simpleVideo.localStream);
                    call.on('stream', (remoteStream) => {
                        dotNetRef.invokeMethodAsync('OnRemoteStreamAdded', call.peer);
                        setTimeout(() => window.simpleVideo.attachStream(call.peer, remoteStream), 100);
                    });
                    window.simpleVideo.activeCalls[call.peer] = call;
                });
                resolve(id);
            });
        });
    },

    startLocalStream: async (initMuted, initVideoOff) => {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
            window.simpleVideo.localStream = stream;

            if (initMuted) stream.getAudioTracks().forEach(t => t.enabled = false);
            if (initVideoOff) stream.getVideoTracks().forEach(t => t.enabled = false);
            window.simpleVideo.attachLocalVideo();

            return true;
        } catch (err) {
            console.error("Camera access denied:", err);
            return false;
        }
    },

    attachLocalVideo: () => {
        const videoEl = document.getElementById("localVideo");
        if (videoEl && window.simpleVideo.localStream) {
            videoEl.srcObject = window.simpleVideo.localStream;
            videoEl.muted = true;
        }
    },

    callUsers: (peerIds) => {
        peerIds.forEach(peerId => {
            const call = window.simpleVideo.peer.call(peerId, window.simpleVideo.localStream);
            call.on('stream', (remoteStream) => {
                window.simpleVideo.activeCalls[peerId] = call;
                setTimeout(() => window.simpleVideo.attachStream(peerId, remoteStream), 50);
                setTimeout(() => window.simpleVideo.attachStream(peerId, remoteStream), 500);
            });
        });
    },

    attachStream: (peerId, stream) => {
        const el = document.getElementById(`video-${peerId}`);
        if (el) el.srcObject = stream;
    },

    toggleAudio: (isEnabled) => {
        if (window.simpleVideo.localStream) {
            window.simpleVideo.localStream.getAudioTracks().forEach(t => t.enabled = isEnabled);
        }
    },

    toggleVideo: (isEnabled) => {
        if (window.simpleVideo.localStream) {
            window.simpleVideo.localStream.getVideoTracks().forEach(t => t.enabled = isEnabled);
        }
    },

    startScreenShare: async () => {
        try {
            const screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true });
            const screenTrack = screenStream.getVideoTracks()[0];

            const localVideo = document.getElementById("localVideo");
            if (localVideo) localVideo.srcObject = screenStream;

            for (let peerId in window.simpleVideo.activeCalls) {
                const call = window.simpleVideo.activeCalls[peerId];
                const sender = call.peerConnection.getSenders().find(s => s.track.kind === 'video');
                if (sender) sender.replaceTrack(screenTrack);
            }

            screenTrack.onended = () => {
                window.simpleVideo.stopScreenShare();
            };

            return true;
        } catch (err) {
            console.error("Screen share cancelled", err);
            return false;
        }
    },

    stopScreenShare: async () => {
        const camStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
        const camTrack = camStream.getVideoTracks()[0];

        const localVideo = document.getElementById("localVideo");
        if (localVideo) localVideo.srcObject = camStream;

        for (let peerId in window.simpleVideo.activeCalls) {
            const call = window.simpleVideo.activeCalls[peerId];
            const sender = call.peerConnection.getSenders().find(s => s.track.kind === 'video');
            if (sender) sender.replaceTrack(camTrack);
        }

        const oldTracks = window.simpleVideo.localStream.getVideoTracks();
        window.simpleVideo.localStream.removeTrack(oldTracks[0]);
        window.simpleVideo.localStream.addTrack(camTrack);
    },

    leave: () => {
        if (window.simpleVideo.localStream) {
            window.simpleVideo.localStream.getTracks().forEach(track => track.stop());
            window.simpleVideo.localStream = null;
        }
        if (window.simpleVideo.peer) {
            window.simpleVideo.peer.destroy();
            window.simpleVideo.peer = null;
        }
        window.simpleVideo.activeCalls = {};
    }
};