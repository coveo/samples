/* jshint undef:true, eqeqeq:true, curly:true, strict:true, laxbreak: true, laxcomma: true */
/* global Coveo,window */

(function($,_,undefined){
  "use strict";
  // Related Results Using GroupByQuery
  // Parameters:
  // {
  //   searchInterface:"#search", // The coveo Search Interface selector
  //   containerselector:"",      // The container for storing your results
  //   templateSelector:"",       // The template for each result shown
  //   remoteQuery:"",            // The query to get your related elements
  //   remoteField:"",            // The field on your related element which to match
  //   queryField:"",             // The field on your local elements which will match remote (related) elements
  //   numberOfResults:5,         // The number of Related results you want
  //   debug : false              // If you want some debug output
  //  }
  function CoveoRelatedResults(options){
    if(!(this instanceof CoveoRelatedResults)){
      return new CoveoRelatedResults();
    }

    var opts = _.defaults(options, {
      searchInterface: "#search",
      numberOfResults: 5,
      debug : false
    });

    var groupByIndex=-1;
    var lastGroupByResults = [];
    // Get the related elements in the current Query by appending
    // to groupBy and getting new results (like facets)
    $(opts.searchInterface).on(Coveo.Events.QueryEvents.doneBuildingQuery, function(e,args){
      groupByIndex = args.queryBuilder.groupByRequests.length;
      args.queryBuilder.groupByRequests.push({
        field: opts.queryField
      , sortCriteria : "ChiSquare"
      , maximumNumberOfValues : opts.NumberOfResults
      , injectionDepth: 1000
      });
    });

    // Once the current Query is done Make a new Query to get
    // the related results themselves
    $(opts.searchInterface).on(Coveo.Events.QueryEvents.querySuccess,function(e,args){

      if( groupByIndex === -1 || args.results.groupByResults === 0 ){
        $(opts.containerSelector).empty();
        return;
      }

      var groupByResults = args.results.groupByResults[groupByIndex].values;

      var haveNewResults = _.some(groupByResults,function(result,i){
        return !_.contains(lastGroupByResults,result.value);
      });
      var lastGroupByResult = _.map(groupByResults,function(result){return result.value;});

      // We have new results, lets make a new query
      if(haveNewResults){
        var queryBuilder = new Coveo.Ui.QueryBuilder();

        if(opts.remoteQuery){queryBuilder.advancedExpression.add(opts.remoteQuery);}
        queryBuilder.advancedExpression.addFieldExpression(opts.remoteField, '==', _.map(groupByResults, function(result){return result.value;}));

        Coveo.Rest.SearchEndpoint.endpoints["default"].search(queryBuilder.build()).done(function(data) {
          var resultsMap = {};

          _.forEach(data.results,function(result){
            resultsMap[result.raw[opts.remoteField.substr(1)]] = result;
          });

          var container = $(opts.containerSelector).empty();
          var template = _.template($(opts.templateSelector).text());

          _.forEach(groupByResults,function(result){
            container.append(template({
              groupBy:result,
              result:resultsMap[result.value]
            }));
          });
        });
      }
    });
  }
  window.CoveoRelatedResults = CoveoRelatedResults;
})(Coveo.$,Coveo._,undefined);
