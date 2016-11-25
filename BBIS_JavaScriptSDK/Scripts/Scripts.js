'use strict';

// Create our Angular application
angular.module('bbADF', ['ui.bootstrap'])
.controller('DonationController', function ($scope, CountryService) {

    // Controls the available donor types
    $scope.defaults = {
        type: '', // single | recurring
        payments: '', // credit | later
        amount: 0 // dollar amount | other
    };

    $scope.types = [
      {
          id: 'single',
          label: 'Single Gift'
      },
      {
          id: 'recurring',
          label: 'Recurring Gift'
      }
    ];

    $scope.payments = [
      {
          id: 'credit',
          label: 'Credit Card'
      },
      {
          id: 'later',
          label: 'Bill Me Later'
      }
    ];

    $scope.amounts = [
      {
          amount: 5,
          label: '$5'
      },
      {
          amount: 10,
          label: '$10'
      },
      {
          amount: 25,
          label: '$25'
      },
      {
          amount: 'other',
          label: 'Other'
      }
    ];

    $scope.designations = [
      {
          id: '0001',
          label: 'Designation 1'
      },
      {
          id: '0002',
          label: 'Designation 2'
      }
    ];

    // Calendar manipulation
    $scope.minDate = new Date();
    $scope.open = function ($event, opened) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope[opened] = true;
    };

    // Temporarily debugging
    $scope.log = function () {
        console.log(this);
    };

    // Request the states for a particular country
    $scope.getStates = function () {
        CountryService.getStates($scope.Donor.Address.Country.alpha3_code).success(function (data) {
           $scope.states = data.RestResponse.result;
        }).error(function () {
            alert('Error loading states.');
        });
    }

    CountryService.getCountries().success(function (data) {
      $scope.countries = data.RestResponse.result;
    }).error(function () {
        alert('Error loading countries.');
    });

})
.service('CountryService', function ($http) {

    // Baseurl when using the Advanced Donation Form API
   
    var baseurl = 'http://services.groupkt.com/';
   

    // Request countries
    this.getCountries = function () {
        debugger;
        return $http.get(baseurl + 'country/get/all');
    }

    // Request states when a country is selected
    this.getStates = function (id) {
        debugger;
        var url = baseurl + '/state/get/' + id + '/all';
        return $http.get(url);
    };

});