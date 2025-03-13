var initialItemList = [];
document.addEventListener("DOMContentLoaded", function () {
    var items = document.getElementsByClassName("items");
    initialItemList = Array.from(items).map(item => item.cloneNode(true));
});
function applyFilters() {
    var dropdown = document.getElementById("dropdownId");
    var selectedOption = dropdown.options[dropdown.selectedIndex].value;
    var favoriteCheckbox = document.getElementById("favoriteCheckbox").checked;

    var items = document.getElementsByClassName("items");
    var filteredItems = Array.from(items);

    if (favoriteCheckbox) {
        filteredItems = filteredItems.filter(item => {
            var favoriteItemCheckbox = item.querySelector(".favorite-item");
            return favoriteItemCheckbox && favoriteItemCheckbox.checked;
        });
    }

    if (selectedOption === "name") {
        filteredItems.sort((a, b) => {
            return a.querySelector("h3").textContent.localeCompare(b.querySelector("h3").textContent);
        });
    } else if (selectedOption === "price") {
        filteredItems.sort((a, b) => {
            var priceA = parseFloat(a.querySelector(".item-price").textContent);
            var priceB = parseFloat(b.querySelector(".item-price").textContent);
            return priceA - priceB;
        });
    }

    var content = document.querySelector(".content");
    content.innerHTML = "";
    filteredItems.forEach(item => {
        content.appendChild(item);
    });
}

function clearFilters() {
    var content = document.querySelector(".content");
    content.innerHTML = "";

    initialItemList.forEach(item => content.appendChild(item.cloneNode(true)));
}
