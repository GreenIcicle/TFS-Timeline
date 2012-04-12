var tfsTimeline = tfsTimeline || {};

tfsTimeline.BuildsViewModel = function (serviceUrl) {
    var self = this;

    // Initial URL to retrieve data from
    self.serviceUrl = serviceUrl;

    // Observable array of build runs
    self.builds = ko.observableArray([]);

    // Indicates that data is still loading,
    // initialize with true until data _has_been_ loaded.
    self.isLoading = ko.observable(true);

    // merges a build into the list of build view models
    self.mergeBuildIntoList = function (newBuild) {
        var currentBuild,
            isNew = true;

        for (var index = 0; index < self.builds().length; index++) {
            currentBuild = self.builds()[index];
            if (currentBuild.Uri() == newBuild.Uri) {
                isNew = false;
                self.applyToBuildViewModel(newBuild, currentBuild);
            }
        }

        // create a new view model and insert it at the top of the list
        if (isNew == true) {
            self.builds.splice(0, 0, self.createBuildViewModel(newBuild));
        }
    };

    // Copies all properties from a normal build to an observble viewmodel
    self.applyToBuildViewModel = function (build, buildViewModel) {
        for (var prop in build) {
            buildViewModel[prop](build[prop]);
        }
    };

    // Creates a new observable Build object
    self.createBuildViewModel = function (build) {

        var buildViewModel = {};

        // Copy all properties from model to viewmodel
        for (var prop in build) {
            buildViewModel[prop] = ko.observable(build[prop]);
        }

        // display FxCop section only if there are either warnings or errors, 
        // hide when there's nothing to show
        buildViewModel.showFxCop = ko.computed(function () {
            return ((buildViewModel.CodeAnalysisErrors() + buildViewModel.CodeAnalysisWarnings()) > 0);
        });

        // indicate that the build is currently running
        buildViewModel.isRunning = ko.computed(function () {
            return buildViewModel.Status() == "Running";
        });

        // Counts how long the build is running (in seconds)
        buildViewModel.runDuration = ko.observable(0);

        // Formats the information how long the build is running as m:ss
        buildViewModel.runDurationMinutes = ko.computed(function () {
            var duration = buildViewModel.runDuration();
            return tfsTimeline.formatDuration(duration);
        });

        // Formats the time that the build has been started at as hh:mm
        buildViewModel.startedAtTime = ko.computed(function () {
            var startedAt = tfsTimeline.parseJsonDate(buildViewModel.StartedAt());
            return tfsTimeline.formatShortTime(startedAt);
        });

        return buildViewModel;
    };

    // after a new item is added to the list, fade it in
    self.handleAfterAdd = function (domNode) {
        $(domNode).fadeIn(1000);
    };

    // load data from the REST service 
    self.refresh = function () {
        $.ajax({
            type: "POST",
            url: self.serviceUrl,
            data: "",
            dataType: "json"
        }).done(function (response) {
            self.updateBuildList(response);

            // re-fetch data after 15 seconds
            tfsTimeline.setTimeout('tfsTimeline.buildsViewModel.refresh();', 15000);
        });
    };

    // updates the build list with a list of builds that have been 
    // passed from the web service.
    self.updateBuildList = function (buildList) {
        // merge each returned build item into the view model list
        $.each(buildList.Builds, function (index, build) {
            self.mergeBuildIntoList(build);
        });

        // update the service url with the URL to refresh the current build 
        // list with
        self.serviceUrl = buildList.Refresh;
        self.isLoading(false);
    };

    // As long as the build is running, update the build duration.
    self.updateBuildDuration = function (currentTime) {
        $.each(self.builds(), function (index, build) {
            if (build.isRunning()) {
                var startedAt = tfsTimeline.parseJsonDate(build.StartedAt());
                var duration = Math.abs(currentTime - startedAt) / 1000;
                build.runDuration(duration);
            }
        });
        tfsTimeline.setTimeout('tfsTimeline.buildsViewModel.updateBuildDuration(new Date())', 1000);
    };
};

