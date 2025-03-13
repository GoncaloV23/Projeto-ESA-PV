function toggleClientLists(action) {
    if (action === 'add') {
        document.getElementById('currentClientsDropdown').style.display = 'none';
        document.getElementById('allClientsDropdown').style.display = 'block';
    } else if (action === 'remove') {
        document.getElementById('allClientsDropdown').style.display = 'none';
        document.getElementById('currentClientsDropdown').style.display = 'block';
    }
}

function cancelSelection() {
    var radios = document.getElementsByName('clientAction');
    for (var i = 0; i < radios.length; i++) {
        radios[i].checked = false;
    }

    document.getElementById('employeeDropdown').selectedIndex = 0;

    document.getElementById('allClientsDropdown').style.display = 'none';
    document.getElementById('currentClientsDropdown').style.display = 'none';
}


