
function viewModel(data) {
    //this.profiles = ko.observableArray(data.profiles.Items);
    this.selectedOptions = ko.observableArray();
    this.StartDate = ko.observable();
    this.EndDate = ko.observable();
    var mappedProfiles = $.map(data.profiles.Items, function(list) { return new createProfile(list); });
    this.profiles = ko.observableArray(mappedProfiles);
    this.jsondaylist = ko.observableArray();
    this.getReport = function(id) {
        $('#report-content').empty();
        $.getJSON("/Profile/Detail?id=" + id, function(data) {
            var items = [];

            $.each(data, function(key, val) {

                if (key == "citylist") {
                    items.push('<span>"' + key + '"</span><li style="color:red;" id="' + key + '">' + val + '</li><br/>');
                } else if (key == "topKeys") {
                    var topKeywords = val;
                    $.each(topKeywords, function(keyu, valu) {
                        items.push('<span>"' + keyu + '"</span><li style="color:purple;" id="' + keyu + '">' + valu + '</li><br/>');
                    });

                } else {
                    items.push('<span>"' + key + '"</span><li id="' + key + '">' + val + '</li><br/>');
                }
            });

            $('<ul/>', {
                'class': 'my-new-list',
                html: items.join('')
            }).appendTo('#report-content');
        });
    };

    this.getAnalyticsReport = function (id, url, StartDate, EndDate) {
        $.post('/Profile/Report', {
            name: "test",
            id: id[0],
            url: url,
            startDate: StartDate,
            endDate: EndDate
        }).done(function (data) {
            $('#report-content').empty();
            $('#report-content').append(data);
        })
          .fail(function(xhr) {
              alert(xhr.status);
              $('#report-content').empty();
              $('#report-content').append(xhr.responseText);
          });
    }
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