namespace DotNetApi;

record TestRequest(DateTime SentDate);

record TestResponse(DateTime SentDate, DateTime ReceivedDate);
