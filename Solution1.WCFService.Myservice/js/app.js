var ClickCounterViewModel = function () {

    this.ChartWidth = ko.observable(0);
    this.dateBegin = ko.observable("2022-01-03T12:00:00Z");
    this.dateEnd = ko.observable("2022-01-03T14:00:00Z");
    this.selectedAlgorithm = ko.observable("");

    this.Algorithms = ko.observableArray([]);
    this.DataArray = ko.observableArray([]);
    this.ExecTimeArray = ko.observableArray();

    this.Scale = ko.pureComputed(() => {
        dateB = new Date(this.dateBegin());
        dateE = new Date(this.dateEnd());
        return Math.round((dateE - dateB) / this.ChartWidth());
    });

    this.ExecutionTime = ko.pureComputed(() => {
        var ret = 0.0;
        this.ExecTimeArray().forEach(time => {
            ret += time;
        });
        return (this.ExecTimeArray().length == 0) ? -1 : ret / this.ExecTimeArray().length;
    });

    this.RequestHeaders = {
        "Content-Type": "application/json",
        "Accept": "application/json",
    };

    this.ChartOptions = {
        responsive: true,
        onResize: (chart, size) => {
            this.ChartWidth(size.width);
        },
        animation: false,
        observeChanges: true,
        interaction: {
            mode: 'index',
            intersect: false,
        },
        elements: {
            line: { borderWidth: 1 },
            point: { radius: 0 }
        },
        parsing: { xAxisKey: 'Date' },
        plugins: { legend: { display: false } },
    };

    this.ChartData = ko.pureComputed(() => {
        let items = this.DataArray().slice().sort((a, b) => (a.Date - b.Date));
        return {
            labels: items.map(i => i.Date.format("HH-MM-ss")),
            datasets: [
                { label: 'Temp', data: items, parsing: { yAxisKey: 'Temp' }, borderColor: 'green' }
            ]
        };
    });

    this.GetData = function () {
        this.ExecTimeArray().length = 0;
        for (let i = 0; i < 5; i++) {
            fetch("http://localhost:50343/Service2.svc/dan?" +
                "dateBegin=" + this.dateBegin() +
                "&dateEnd=" + this.dateEnd() +
                "&algorithm=" + this.selectedAlgorithm() +
                "&scale=" + this.Scale(),
                { headers: this.RequestHeaders })
                .then(response => response.json())
                .then(data => {
                    data.MeteoData.forEach(i => {
                        i.Date = new Date(i.Date);
                    });
                    this.DataArray(data.MeteoData);
                    this.ExecTimeArray.push(data.ExecutionTime);
                    console.log(this.ExecTimeArray());
                    //this.ExecutionTime(data.ExecutionTime);
                })
        }
    };

    //receive Algorithms
    this.GetAlgorithms = function () {
        fetch("http://localhost:50343/Service2.svc/algorithms",
            { headers: this.RequestHeaders })
            .then(response => response.json())
            .then(data => this.Algorithms(["", ...data]));
    };
};
