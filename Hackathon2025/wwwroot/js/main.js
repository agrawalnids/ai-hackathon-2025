function getCookie(name) {
    const cookieString = document.cookie;
    const cookies = cookieString.split(';');

    for (let i = 0; i < cookies.length; i++) {
        const cookie = cookies[i].trim();
        if (cookie.startsWith(name + '=')) {
            return cookie.substring(name.length + 1);
        }
    }
    return null;
}

function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + encodeURIComponent(value) + expires + "; path=/";
}

function updateBackgroundColor(color) {
    if (color) {
        document.body.style.backgroundColor = color;
    }
}

function playMoodAudio(playOrStop) {

    if (playOrStop == 'play') {
        //as noted in addendum, check for querystring exitence
        var symbol = $(".mood-music-wrp iframe")[0].src.indexOf("?") > -1 ? "&" : "?";

        $(".mood-music-wrp").data('link', $(".mood-music-wrp iframe")[0].src);

        //modify source to autoplay and start video
        $(".mood-music-wrp iframe")[0].src += symbol + "autoplay=1";
    } else {
        var iframe = $(".mood-music-wrp iframe")[0];
        if (iframe) {
            iframe.src = $(".mood-music-wrp").data('link');
        }
    }


    $(".mood-music-wrp button.__play").toggleClass('__hide');
    $(".mood-music-wrp button.__stop").toggleClass('__hide');
}


function hideLoader() {
    $('.loader').removeClass('__active');
}

function startTimer() {
    const countdownElement = document.getElementById('timer'); // Ensure this element exists in your HTML
    const duration = 5 * 60; // 5 minutes in seconds
    let remainingTime = duration;

    const timerInterval = setInterval(() => {
        const minutes = Math.floor(remainingTime / 60);
        const seconds = remainingTime % 60;

        // Update the countdown display
        countdownElement.textContent = `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;

        if (remainingTime <= 0) {
            clearInterval(timerInterval);
            // Optional: Add any action to perform when the timer ends
            //alert('Time is up!');
        }

        remainingTime--;
    }, 1000);
}