# Region_201910.db

基于 国家统计局 2019 年 10 月 31 日 数据
http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2019/index.html

## JsonRegion.json

省市区（不包含街道/乡镇） 的 JSON 数据

## region_201910_all.sql

省市区街道 SQL 数据

## region_201910.sql

省市区街道 SQL 数据 相比 `region_201910_all.sql` , 移除市下面，市辖区区域，共 280 个； 移除 Status 字段


### Status 字段含义
```
/// 1 正常可用
/// 2 修改名称，直辖市 的二级区域从 市辖区 改为 父级名称
/// 3 没有该级区域，手动维护的
/// 0 市辖区这种 或者 没有自己区域的
```
