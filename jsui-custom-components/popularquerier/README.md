### CoveoPopularQuerier

A component that queries and doesnt show results, instead it triggers and event (`dataloaded`)
which gives you an array of objects:

```json
[
 ...,
 {
 "field": "@txtannovtheme",
 "value": "Inspection",
 "score": 18172
  },
  ...
]
```

Usage (with jquery):

```js
$("#popularquerier").on("dataloaded",function(e, results, query){
  // DO stuff
  console.log(results,query)
})
```
