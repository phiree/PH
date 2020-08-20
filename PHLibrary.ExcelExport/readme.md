### 功能: 
将数据导出成excel文件
    
### 特点: 
- 支持多种数据源
    - dataset 和 datatable
    - 对象列表IList<T>
    - 匿名对象列表
- 支持多层表头,表头合并
- 支持自定义标题

### 使用方式

- 导出DataTable和DataSet

```
Stream Create(DataTable dataTable);
Stream Create(DataSet dataSet);
```

- 导出对象(包含匿名对象)列表

```
Stream Create<T>(IList<T> data, IDictionary<string, string> propertyNameMaps=null);
```

- 生成多层/合并表头的excel

``` 
Stream Create(DataTable dataTable,ColumnTree columnTree)
```
![image](https://user-images.githubusercontent.com/277597/90484228-78ef0480-e168-11ea-8950-1b15f7849d73.png)
