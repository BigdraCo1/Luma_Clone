lucide.createIcons();

document.querySelector('.profile').addEventListener('click', function () {
    this.classList.toggle('active');
});

document.addEventListener('click', function (event) {
    const profile = document.querySelector('.profile');
    if (!profile.contains(event.target)) {
        profile.classList.remove('active');
    }
});

window.addEventListener("scroll", function () {
    const navbar = document.querySelector(".navbar");
    if (window.scrollY > 0) {
        navbar.classList.add("scrolled");
    } else {
        navbar.classList.remove("scrolled");
    }
});