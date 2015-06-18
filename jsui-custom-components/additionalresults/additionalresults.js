;(function(Coveo,$,_,undefined){
  "use strict";

  var getParentSearchInterface = function(e){
    var jqe = $(e);

    if(jqe.parent('.CoveoSearchInterface').length===0){
      return getParentSearchInterface(jqe.parent()[0]);
    }

    return jqe.parent('.CoveoSearchInterface')[0];
  };

  var CoveoAdditionalResults = function(element,options){
    if(!(this instanceof CoveoAdditionalResults)){
      return new CoveoAdditionalResults(element,options);
    }

    var searchInterface = getParentSearchInterface(element);
    var template = _.template($(options.templateSelector).text());

    $(searchInterface).on(Coveo.Events.QueryEvents.doneBuildingQuery, function(e, args) {
      var newQuery = new Coveo.Ui.QueryBuilder();

      if(options.hiddenExpression && options.hiddenExpression !== ""){
        newQuery.advancedExpression.add(options.hiddenExpression);
      }

      Coveo.Rest.SearchEndpoint.endpoints[options.endpoint].search(newQuery.build()).done(function(data) {
        var container = $(element).empty();
        container.append(template({results:data.results}));
      });

    });

  };


  // Initialize from divComponent
  // <div class="CoveoAdditionalResults"
  //      data-template-selector="#poc-result-list-template"
  //      data-hidden-expression="@objecttype==POCC"></div>
  $(function(){
    _.each($('.CoveoAdditionalResults'),function(resultElement){
      new CoveoAdditionalResults(resultElement,{
        endpoint:$(resultElement).attr('data-endpoint') || "default",
        templateSelector:$(resultElement).attr('data-template-selector'),
        hiddenExpression:$(resultElement).attr('data-hidden-expression')
      });
    });
  });

  window.CoveoAdditionalResults = CoveoAdditionalResults;
})(Coveo,Coveo.$,Coveo._);
