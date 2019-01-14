import {
  Component,
  ComponentOptions,
  IComponentBindings,
  QueryEvents,
  IBuildingQueryEventArgs,
  Initialization,
} from 'coveo-search-ui';

export interface IItemGroupingOptions {
  fieldToGroup: string;
  fieldForRanking: string;
}

export class ItemGrouping extends Component {
  static ID = 'ItemGrouping';

  static options: IItemGroupingOptions = {

    fieldToGroup: ComponentOptions.buildStringOption({
      defaultValue: '@groupid', 
    }),

    fieldForRanking: ComponentOptions.buildStringOption({
      defaultValue: '@color'
    })
  };

  constructor(public element: HTMLElement, public options: IItemGroupingOptions, public bindings: IComponentBindings) {
    super(element, ItemGrouping.ID, bindings);
    this.options = ComponentOptions.initComponentOptions(element, ItemGrouping, options);
    this.bind.onRootElement(QueryEvents.buildingQuery, (args: IBuildingQueryEventArgs) => this.handleBuildingQuery(args));
    this.bind.onRootElement(QueryEvents.doneBuildingQuery, (args: IBuildingQueryEventArgs) => this.handleDoneBuildingQuery(args));
  }
  
  // Define the field to be used for grouping
  private handleBuildingQuery(args: IBuildingQueryEventArgs) {
    args.queryBuilder.filterField = this.options.fieldToGroup;
    args.queryBuilder.maximumAge = 0;
  }


  // If there is a keyword, a nested query is used to find the item 
  // and retreive all the related items to create the group. A ranking expression is
  // used to change the order of the items within a group, and the query is removed from
  // the expression since it's injected in the nested query already.
  private handleDoneBuildingQuery(args: IBuildingQueryEventArgs){
    if (!args.queryBuilder.expression.isEmpty()){
      args.queryBuilder.advancedExpression.add("[[" + this.options.fieldToGroup + "] " + args.queryBuilder.expression.build() + "]")
      this.addKeywordsToQRE(args);
      args.queryBuilder.expression.remove(args.queryBuilder.expression.build());
    }
  }

  private addKeywordsToQRE(args: IBuildingQueryEventArgs){
    const words = args.queryBuilder.expression.build().split(" ");
    words.forEach(element => {
      args.queryBuilder.advancedExpression.add("$qre(expression:'" + this.options.fieldForRanking + "=" + element + "', modifier:'15')")
    });
  }

}

Initialization.registerAutoCreateComponent(ItemGrouping);
