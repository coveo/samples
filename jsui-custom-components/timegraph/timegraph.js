/* jshint undef:true, eqeqeq:true, curly:true, strict:true, laxbreak: true, laxcomma: true */
/* global Coveo, window, MG */

// <script type="text/javascript" src="metrics-graphics/metricsgraphics.js"></script>
// <div id="chart"></div>

// CoveoTimeGraph
(function(Coveo, $, _) {
  "use strict";

  // This parses the serverReceived Date
  var parseDate = function(date) {
    var d = _.map(date.split('..'), function(date) {
      return Date.parse(date, 'yyyy/mm/dd@hh:mm:ss');
    });
    return new Date(d[1] - (d[1] - d[0]) / 2);
  };

  var groupByTemplate = {
    field: "@sysdate"
    , maximumNumberOfValues: 40
    , sortCriteria : "nosort"
    , injectionDepth: 1000
    , queryOverride : ""
    , generateAutomaticRanges: true
  };

  var formatGroupBy = function(theme,query){
    var gb = _.clone(groupByTemplate);

    if(theme.query){
      gb.queryOverride = theme.query;
    }else{
      gb.queryOverride = ""+theme.field+"==\"" + theme.label + "\"";
    }
    if(query.aq){
      gb.queryOverride += " AND (" + query.aq +")";
    }

    return gb;
  };

  var CoveoTimeGraph = function(opts){
    this.themes = opts.themes;
    this.update();
  };

  CoveoTimeGraph.prototype.themeLabels = function(){
    return _.map(this.themes,function(theme){ return theme.label; });
  };


  CoveoTimeGraph.prototype.update =function(lastQuery){
    var q = new Coveo.Ui.QueryBuilder();
    q.numberOfResults = 0;

    var themes = this.themes;
    this.lastQuery = this.lastQuery || lastQuery;

    _.forEach(this.themes,function(theme){
      q.groupByRequests.push(formatGroupBy(theme,lastQuery));
    });

    Coveo.Rest.SearchEndpoint.endpoints["default"].search(q.build()).done(function(data){
      var values = data.groupByResults;
      var processedData = _.map(values, function(resultGroup) {
        return _.map(resultGroup,function(result){
          return {
            numberOfResults: result.numberOfResults,
            date: parseDate(result.value),
            nonParsedDate: result.value
          };
        });
      });

      // console.log(processedData)

      MG.data_graphic({
        data: processedData,
        legend: this.themeLabels(),
        legend_target: '.legend',
        width: 600,
        height: 250,
        target: '#chart',
        x_accessor: 'date',
        y_accessor: 'numberOfResults',
      });
    });
  };


  //   var a = new CoveoTimeGraph({
  //     themes:[{
  //       label:"Total",
  //       query:"@URI"
  //       }],
  //   });
  // To update graph:
  //
  //   a.update()

  window.CoveoTimeGraph = CoveoTimeGraph;
})(Coveo, Coveo.$,Coveo._);
