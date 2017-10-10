app.config(function ($stateProvider, $urlRouterProvider) {
    $stateProvider
        .state('login', {
            templateUrl: "login.html",
            controller: 'LoginController'
        })

        $urlRouterProvider.otherwise('/');
});