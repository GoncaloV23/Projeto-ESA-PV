var slideIndex = 0;



function showSlides() {
    var slides = document.getElementsByClassName("slide");
    var dots = document.getElementsByClassName("dot");
    if (slides.length === 0 || dots.length === 0) {
        return;
    }
    for (var i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    slideIndex++;
    if (slideIndex > slides.length) { slideIndex = 1; }
    for (var i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    slides[slideIndex - 1].style.display = "block";
    dots[slideIndex - 1].className += " active";
    setTimeout(showSlides, 2000);
}

document.addEventListener("DOMContentLoaded", function () {
    showSlides();

    var slides = document.querySelectorAll('.slide');
    var formBox = document.querySelector('.form-box');
    var dotsContainer = document.querySelector('.dots-container');

    var slideHeight = slides[0].clientHeight;
    formBox.style.height = slideHeight + 'px';

    var dotsContainerTop = slideHeight - dotsContainer.clientHeight;
    dotsContainer.style.top = dotsContainerTop + 'px';
});