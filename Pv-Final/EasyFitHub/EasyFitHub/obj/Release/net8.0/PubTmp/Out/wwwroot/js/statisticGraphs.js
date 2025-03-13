function loadGymCount(elementId, title) {
    loadGraphByField(elementId, title, "gymCount")
}
function loadUserCount(elementId, title) {
    loadGraphByField(elementId, title, "userCount")
}
function loadAvgAge(elementId, title) {
    loadGraphByField(elementId, title, "avgAge")
}

function loadGraphByField(elementId, title, field) {
    loadGraph(
        elementId,
        title,
        stats.map(e => (new Date(e.theDate).toLocaleDateString('pt-BR'))),
        stats.map(e => e[field])
    );
}
function loadLineChartByField(elementId, title, field) {
    loadLineChart(
        elementId,
        title,
        stats.map(e => (new Date(e.theDate).toLocaleDateString('pt-BR'))),
        stats.map(e => e[field])
    );
}

function loadTopGyms(elementId, title, index) {
    let topGyms = stats.map(e => e.topGyms);
    let topGym = topGyms[index];

    let labels = topGym.map(e => e.entityKey);
    let values = topGym.map(e => parseFloat(e.entityValue));
    loadGraph(
        elementId,
        title,
        labels,
        values
    );

}
function loadPieChartByField(elementId, title, field, index, colors) {
    let list = stats.map(e => e[field]);
    let entryPair = list[index];

    let labels = entryPair.map(e => e.entityKey);
    let values = entryPair.map(e => parseFloat(e.entityValue.replace(',', '.')));
    loadPieChart(
        elementId,
        title,
        labels,
        values,
        colors
    );

}
function loadSexRates(elementId, title, index) {
    let sexRates = stats.map(e => e.sexRates);
    let sexRate = sexRates[index];

    let labels = sexRate.map(e => e.entityKey);
    let values = sexRate.map(e => parseFloat(e.entityValue.replace(',', '.')));
    loadPieChart(
        elementId,
        title,
        labels,
        values,
        [
            'rgba(54, 162, 235, 1)',
            'rgba(255, 99, 132, 1)',
            'rgba(255, 206, 86, 1)'
        ]//COLORS
    );

}
function loadGraph(elementId, title, labels, values) {
    var element = document.getElementById(elementId).getContext('2d');

    var graph = new Chart(element, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: title,
                data: values,
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });
}
function loadPieChart(elementId, title, labels, values, colors) {
    var element = document.getElementById(elementId).getContext('2d');
    var meuPieChart = new Chart(element, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                label: title,
                data: values,
                backgroundColor: colors,
                borderColor: colors,
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });
}
function loadLineChart(elementId, title, labels, values) {
    var ctx = document.getElementById(elementId).getContext('2d');
    var meuLineChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: title,
                data: values,
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgba(255, 99, 132, 1)',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });
}
function orderAscByDate(list) {
    list.sort((e1, e2) => {
        const date1 = new Date(e1.theDate);
        const date2 = new Date(e2.theDate);

        return date1 - date2;
    });
}
