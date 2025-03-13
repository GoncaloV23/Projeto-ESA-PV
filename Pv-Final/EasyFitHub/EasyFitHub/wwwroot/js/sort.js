function sortGallery(sortBy) {
    var galleries = document.getElementsByClassName("gallery");
    var sortedGalleries = galleries.sort(function (a, b) {
        var valueA, valueB;
        if (sortBy === 'name') {
            valueA = a.getElementsByTagName("p")[0].innerText;
            valueB = b.getElementsByTagName("p")[0].innerText;
        } else if (sortBy === 'quantity') {
            valueA = parseInt(a.getElementsByTagName("p")[1].innerText);
            valueB = parseInt(b.getElementsByTagName("p")[1].innerText);
        }
        return valueA.localeCompare(valueB);
    });
    var contentDiv = document.getElementsByClassName("content");
    contentDiv.innerHTML = "";
    sortedGalleries.forEach(function (gallery) {
        contentDiv.appendChild(gallery);
    });
}