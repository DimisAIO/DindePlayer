var isSliding = false;
var audio = new Audio();
let db = [];
let resultsHTML;

function start(url, title) {
    audio.src = url;
    branding.innerText = `${title}`;
    volume();
    speed();
    applyPitch();
    pauseplay();
}
function pauseplay() {
    if(branding.innerText == "DindePlayer") return;
    if(audio.paused) {
        audio.play();
    } else {
        audio.pause();
    }
    pausePlayButton.innerText = audio.paused ? "▶️" : "⏸️";
}
function volume() {
    audio.volume = vol.value / 100;
    volText.innerText = `(${Math.floor(audio.volume*100)}%)`;
}
function speed() {
    audio.playbackRate = document.getElementById("speed").value / 100;
    speedText.innerText = `(${audio.playbackRate})`;
}
function applyPitch() {
    audio.preservesPitch = !pitch.checked;
    var newPitchText = `(${audio.preservesPitch ? "No" : "Yes"})`;
    pitchText.innerText = newPitchText;
}

function handle(idk) {
    console.log("Loading da files");
    db = [];
    var x = idk.split("|");
    resultsPart.innerHTML = "";
    searchBox.value = "";
    x.forEach(element => {
        var songLink = "file:///" + element.replaceAll("\\", "/");
        var songName = element.slice(0, -4).split("\\");
        songName = songName[songName.length - 1];
        if(songName != "" && songLink != "file:///") {
            db.push({name: songName, link: songLink});
            resultsPart.innerHTML += `<a href="javascript:start('${songLink}', '${songName}')">${songName}</a>`;
        }
    });
    resultsHTML = resultsPart.innerHTML;
}

searchBox.oninput = () => {
    if(searchBox.value.length < 1) return resultsPart.innerHTML = resultsHTML;
    else resultsPart.innerHTML = "";
    db.forEach(element => {
        if (element.name.toLowerCase().includes(searchBox.value.toLowerCase())) {
            resultsPart.innerHTML += `<a href="javascript:start('${element.link}', '${element.name}')">${element.name}</a>`;
        }
    })
}

// Set max value when you know the duration
audio.onloadedmetadata = () => seekbar.max = audio.duration;
// update audio position
seekbar.onchange = () => audio.currentTime = seekbar.value;
// update range input when currentTime updates
audio.ontimeupdate = () => {
    if(isSliding) return;
    seekbar.value = audio.currentTime;
    seekbarText.innerText = new Date(Math.round(seekbar.value) * 1000).toISOString().slice(11, 19);
    if(seekbar.value == seekbar.max) pausePlayButton.innerText = "▶️";
}


seekbar.addEventListener('input', function () {
    isSliding = true;
    seekbarText.innerText = new Date(Math.round(seekbar.value) * 1000).toISOString().slice(11, 19);
});

seekbar.addEventListener('mouseup', function () {
    seekbar.onchange();
    isSliding = false;
    audio.ontimeupdate();
});

document.addEventListener('keydown', function (event) {
    if(document.activeElement.id == "searchBox" || document.activeElement.id == "pitch") return; // Avoid to pause when searching;
    if(event.keyCode == 32) {
        event.preventDefault(); // I never understood this, but it works!
        pauseplay();
    }
});