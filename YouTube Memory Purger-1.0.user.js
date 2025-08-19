// ==UserScript==
// @name         YouTube Memory Purger
// @namespace    http://tampermonkey.net/
// @version      1.0
// @description  Recarga la pestaña de YouTube cada X videos para purgar memoria.
// @author       Gerardo Tordoya
// @match        https://www.youtube.com/*
// @grant        none
// ==/UserScript==

(function() {
    'use strict';

    const RELOAD_EVERY = 3; // recarga cada X videos
    let videoCounter = 0;

    function getVideoElement() {
        return document.querySelector('video');
    }

    function getPlaylistParams() {
        const urlParams = new URLSearchParams(window.location.search);
        const list = urlParams.get('list');
        const index = urlParams.get('index');
        return { list, index };
    }

    function reloadPage() {
        const { list, index } = getPlaylistParams();
        if (list) {
            // reconstruye la URL para retomar la playlist
            const newUrl = `/watch?list=${list}&index=${index || 0}`;
            window.location.href = newUrl;
        } else {
            window.location.reload();
        }
    }

    const observer = new MutationObserver(() => {
        const video = getVideoElement();
        if (!video) return;

        video.addEventListener('ended', () => {
            videoCounter++;
            if (videoCounter >= RELOAD_EVERY) {
                reloadPage();
            }
        }, { once: true });
    });

    observer.observe(document.body, { childList: true, subtree: true });
})();
