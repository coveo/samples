/* jshint undef:true, eqeqeq:true, curly:true, strict:true, laxbreak: true, laxcomma: true */
/* global Coveo, document,window */

(function($,_,undefined){
  "use strict";
// # CoveoPopularQuerier
//
// A component that queries and doesnt show results, instead it triggers a callback
// which gives you an array of objects:
// ```json
// [
//   ...,
//   {
//     "field": "@txtannovtheme",
//     "value": "Inspection",
//     "score": 18172
//   },
//   ...
// ]
// ```
// And the query that was run to obtain those objects (in case you want to do other queries based on that)


// Options: {
//   searchInterface: "#search"
//   , numberOfResults: 5
//   , sortCriteria: "ChiSquare"
//   , debug : false
//   , opts.queryField : "@thefield"
//   , onLoaded :function(results,queryArgs){} // the callback
// }

function CoveoPopularQuerier(options){
  if(!(this instanceof CoveoPopularQuerier)){
    return new CoveoPopularQuerier();
  }

  var groupByIndex = -1;

  var opts = _.defaults(options, {
    searchInterface: "#search"
  , numberOfResults: 5
  , sortCriteria: "ChiSquare"
  , debug : false
  });

  $(opts.searchInterface).on(Coveo.Events.QueryEvents.doneBuildingQuery, function(e,args){
    groupByIndex = args.queryBuilder.groupByRequests.length;

    var groupByReq = {
        field: opts.queryField
      , sortCriteria: opts.sortCriteria
      , injectionDepth: opts.injectionDepth
    };
    args.queryBuilder.groupByRequests.push(groupByReq);
  });

  $(opts.searchInterface).on(Coveo.Events.QueryEvents.querySuccess,function(e,args){
    if( groupByIndex === -1 || args.results.groupByResults === 0 ){
      return;
    }

    var results = _.map(args.results.groupByResults,function(v){
      return {
        field:opts.queryField.substr(1),
        value:v.value,
        score:v.numberOfResults
      };
    });

      opts.onLoaded(results,args);
  });
}


  // Auto Initializer (from dom-elements)
  // $(function() {
  //   var popularQueriers = document.getElementsByClassName('CoveoPopularQuerier');
  //   _.each(popularQueriers, function(popularQuerier) {
  //     var options = {};
  //
  //     // TODO: We need to transform data-properties to options object
  //     new CoveoPopularQuerier(options);
  //   });
  // });


  window.CoveoPopularQuerier = CoveoPopularQuerier;
})(Coveo.$,Coveo._,undefined);
