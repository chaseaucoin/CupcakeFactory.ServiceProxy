# CupcakeFactory.ServiceProxy
A pain-free way to work with services

[![Build status](https://dev.azure.com/cupcakefactory/ChasesSites/_apis/build/status/CupcakeFactory.ServiceProxy)](https://dev.azure.com/cupcakefactory/ChasesSites/_build/latest?definitionId=12)

Lambda round trip benchmarking results.

|      Method |     Mean |    Error |   StdDev |
|------------ |---------:|---------:|---------:|
| Add2Numbers | 54.35 ms | 1.078 ms | 1.998 ms |


While it's not breaking any records, it's kind of not the point to. This is the same mean and deviation if you have on lambda if you call it directly and just get back a string of "OK";
Why that is important is now you have an easy way to interact with your service that functionally adds no overhead. 

This means that so long as your backend is staying resonable with what it's doing your consumers can feel "real-time" in a cost effective way

https://www.nngroup.com/articles/response-times-3-important-limits/