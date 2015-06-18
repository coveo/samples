/* jshint undef:true, eqeqeq:true, curly:true, strict:true, laxbreak: true, laxcomma: true */
/* global Coveo, window*/

(function($, _, undefined) {
  "use strict";
  // Related Results Using Query Extensions Language
  // Parameters:
  //{
  // searchInterface:"#search", // The coveo Search Interface selector
  // containerselector:"",      // The container for storing your results
  // templateSelector:"",       // The template for each result shown
  // remoteQuery:"",            // The query to get your related elements
  // remoteField:"",            // The field on your related element which to match
  // queryField:"",             // The field on your local elements which will match remote (related) elements
  // numberOfResults:5
  //}
  function CoveoQEXRelatedResults(options) {
    if (!(this instanceof CoveoQEXRelatedResults)) {
      return new CoveoQEXRelatedResults(options);
    }

    var opts = _.defaults(options, {
      searchInterface: "#search",
      numberOfResults: 5
    });

    $(opts.searchInterface).on(Coveo.Events.QueryEvents.doneBuildingQuery, function(e, args) {
      var q = args.queryBuilder.build();
      var joinedQuery = ["(", q.q, q.aq, q.cq, ")"].join(" ");

      var qel = "{{relatedToFetch=$valuesOfField(field:'" + opts.queryField + "', sortOrder:'SortByChiSquare',resultSet: (" + joinedQuery + ") )}}" + "$joinOnValues(resultSet: " + opts.remoteQuery + ", field:'" + opts.remoteField + "',values:{{relatedToFetch}})";

      var queryBuilder = new Coveo.Ui.QueryBuilder();
      queryBuilder.advancedExpression.add(qel);

      Coveo.Rest.SearchEndpoint.endpoints["default"].search(queryBuilder.build()).done(function(data) {
        var container = $(opts.containerSelector).empty();
        var template = _.template($(opts.templateSelector).text());

        _.forEach(data.results, function(result) {
          container.append(template({
            result: result
          }));
        });
      });
    });
  }

  window.CoveoQEXRelatedResults = CoveoQEXRelatedResults;
})(Coveo.$, Coveo._);
