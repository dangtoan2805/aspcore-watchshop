$(function () {
    /*======== VARIABLE========*/
    //Date defautl
    var start = moment().subtract(7, 'days');
    var end = moment();
    /*======== DEFINE EVENT ON VIEW ========*/
    var onChangeDate = function (start, end) {
        $('#reportrange span').html(start.format(dateFormter) + ' - ' + end.format(dateFormter));
        getReport(start, end);
    }

    /*======== GET DATA ========*/
    function getReport(fromDate, toDate) {
        $.ajax({
            method: "GET",
            url: `/Admin/Home/OrderReport`,
            dataType: "JSON",
            data: { start: fromDate.format('L'), end: toDate.format('L') }
        }).done(data => {
            requestSuccess(data);

        })
    }

    /*======== ATTACH EVENT ========*/
    $('#reportrange').daterangepicker({
        startDate: start,
        endDate: end,
        opens: 'left',
        ranges: {
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    }, onChangeDate);


    /*======== UPDATE VIEW ========*/
    function requestSuccess(data) {
        let lables = []
        let date = []
        let sum = 0;
        let count = 0;
        data.forEach(item => {
            let arDate = item.dateCreated.split('T')[0].split('-');
            lables.push(`${arDate[2]}/${arDate[1]}`);
            date.push(item.countOrder);
            count = count + item.countOrder;
            sum = sum + item.total;
        });
        $('#vt-orders').text(count);
        $('#vt-revenue').text(sum);
        displayReport(lables, date)
    }
    // Display on View
    function displayReport(lables, data) {
        var lineChart = document.createElement('canvas');
        lineChart.classList.add('chartjs');
        $('#chart-content').empty();
        $('#chart-content').append(lineChart);
        if (lineChart !== null) {
            var chart = new Chart(lineChart, {
                // The type of chart we want to create
                type: "line",
                // The data for our dataset
                data: {
                    labels: lables,
                    datasets: [
                        {
                            label: "",
                            backgroundColor: "transparent",
                            borderColor: "rgb(82, 136, 255)",
                            data: data,
                            lineTension: 0.3,
                            pointBackgroundColor: "rgba(255,255,255,1)",
                            pointHoverBackgroundColor: "rgba(255,255,255,1)",
                            pointBorderWidth: 1,
                            pointHoverBorderWidth: 1
                        },
                    ]
                },
                // Configuration options go here
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    borderDashOffset: 0,
                    legend: {
                        display: false
                    },
                    layout: {
                        padding: {
                            right: 10
                        }
                    },
                    scales: {
                        xAxes: [
                            {
                                gridLines: {
                                    display: false
                                }

                            }
                        ],
                        yAxes: [
                            {
                                gridLines: {
                                    display: true,
                                    color: "#eee",
                                    zeroLineColor: "#eee",
                                },
                                ticks: {
                                    min: 0,
                                    suggestedMax: 5,
                                    stepSize: 1
                                }
                            }
                        ]
                    },
                    tooltips: {
                        callbacks: {
                            title: function (tooltipItem, data) {
                                return data["labels"][tooltipItem[0]["index"]];
                            },
                            label: function (tooltipItem, data) {
                                return data["datasets"][0]["data"][tooltipItem["index"]] + " đơn hàng";
                            }
                        },
                        responsive: true,
                        intersect: false,
                        enabled: true,
                        titleFontColor: "#888",
                        bodyFontColor: "#555",
                        titleFontSize: 10,
                        bodyFontSize: 12,
                        backgroundColor: "rgba(256,256,256,0.95)",
                        xPadding: 20,
                        yPadding: 10,
                        displayColors: false,
                        borderColor: "rgba(220, 220, 220, 0.9)",
                        borderWidth: 2,
                        caretSize: 10,
                        caretPadding: 15
                    }
                }
            });
        }
    }

    onChangeDate(start, end);
    /*======== END  ========*/
    $("#loading").fadeOut(500);
});