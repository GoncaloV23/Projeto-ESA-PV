var imgDisplay;
var imgPreview;
var currentIndex = 0;
var removeParam;

function previewImage(event) {
    var input = event.target;

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            imgPreview.src = e.target.result;
        }

        reader.readAsDataURL(input.files[0]);
    } else {
        imgPreview.src = defaultSrc;
    }
}
function showImage(index) {
    imgDisplay.src = images[index].path;
    imgDisplay.alt = images[index].name;

    if (removeParam)removeParam.value = images[index].name;
    
}



function onLoadPage() {
    imgDisplay = document.getElementById("profileImage");
    let prevBtn;
    let nextBtn;
    let addBtn;
    let viewDiv;
    console.log(images);
    try {
        removeParam = document.getElementById("removeParam");
    } catch (error) { console.error(error); }

    

    if (images && images.length>0)showImage(0);

    try {
        prevBtn = document.getElementById("prevBtn");
        nextBtn = document.getElementById("nextBtn");
        prevBtn.addEventListener("click", function () {
            if (currentIndex > 0) {
                currentIndex--;
                showImage(currentIndex);
            }
        });

        nextBtn.addEventListener("click", function () {
            if (currentIndex < images.length - 1) {
                currentIndex++;
                showImage(currentIndex);
            }
        });
    } catch (error) { console.error(error); }

    try {
        addBtn = document.getElementById("addBtn");
        viewDiv = document.getElementById("viewDiv");
        addBtn.addEventListener("click", function () {
            viewDiv.style.display = "none";
            formDiv.style.display = "block";
        });
    } catch (error) { console.error(error); }

    try {
        imgPreview = document.getElementById("preview");
    

        let formDiv = document.getElementById("formDiv");
        let cancelBtn = document.getElementById("cancelBtn");

        cancelBtn.addEventListener("click", function () {
            imgPreview.src = defaultSrc;
            viewDiv.style.display = "block";
            formDiv.style.display = "none";
        });
    } catch (error) { console.error(error); }
}
document.addEventListener("DOMContentLoaded", function () {
    onLoadPage();
    actionAlert();
});