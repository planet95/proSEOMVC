
function viewModel(data) {
    this.profiles = ko.observableArray(data.profiles.Items);
    this.selectedOptions = ko.observableArray();
    this.StartDate = ko.observable();
    this.EndDate = ko.observable();
    var mappedProfiles = $.map(data.profiles.Items, function(list) { return new createProfile(list); });
    this.profiles = ko.observableArray(mappedProfiles);
    this.jsondaylist = ko.observableArray();
    this.getAnalyticsReport = function(id, url, StartDate, EndDate) {
        $.post('/Profile/Report', {
            name: "test",
            id: id[0],
            url: url,
            startDate: StartDate,
            endDate: EndDate
        }).done(function(data) {
            $('#report-content').empty();
            $('#report-content').append(data);
        })
            .fail(function(xhr) {
                alert(xhr.status);
                $('#report-content').empty();
                $('#report-content').append(xhr.responseText);
            });
    };
    this.getPDF = function (id, url, StartDate, EndDate) {
        var $hidData = {
            name: "test",
            id: id[0],
            url: url,
            startDate: StartDate,
            endDate: EndDate
        };

    //    var $hidInput = $('#pdfID');
     //   $hidInput.val($hidData);
    //  $('#ViewPDF').submit();
          $.post('/Profile/ViewPDF', $hidData)
        .done(function (data) {
            var myResponse = data;
            $('#report-content').empty();
            $('#report-content').append('<a href="'+data+'" target="_blank" >PDF</a>');
        })

        .fail(function(xhr) {
            alert(xhr.status);
            $('#report-content').empty();
            $('#report-content').append(xhr.responseText);
        });
    };
}

function createProfile(data) {
    return new Profile(data); // TodoList is injected by model.js
}


function Profile(data) {
    var self = this;
    data = data || {};
    self.name = data.Name;
    self.id = data.Id;
    self.kind = data.Kind;
    self.accountId = data.AccountId;
    self.websiteurl = data.WebsiteUrl.replace('http://www.','');
};

ko.applyBindings(new viewModel(data));