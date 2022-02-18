# Reference implementation (.NET) for DI & UnitTesting - AWS Lambda Functions 
This is a reference implementation for using Dependency Injection in AWS Lambda Functions, specifically for Lambda Functions written in C# for .Net Core runtime. 

## Prerequisites
- Basic knowledge of AWS Lambda functions 
- Practical experience using Dependency Injection in C# / .Net core
- Practical experience writing unit tests in C#

## Overview

A typical AWS Lambda Function written in C#, for .Net Core runtime, has a function handler that looks like:

```
return-type function-handler-name(input-type input, ILambdaContext context) {
   ...
}
```

The function handler is run every time the Lambda Function is invoked, through various events, from different event sources that include SQS, SNS, S3, DynamoDB, API Gateway HTTP integration, etc. 

Since the function handler is where the core logic is implemented, all the dependencies are usually instantiated from here.  This leads to the following issues:
- Unit testing of the function handler is limited; cannot mock behavior of dependent classes
- Execution time of the handler increases If the no. of objects instantiated (happens for each invocation) is more. Since Lambda function executions are billed in milliseconds, this has to be addressed.

This reference implementation addresses both the issues mentioned above.


## Lambda execution environment
The execution environment for a Lambda function is defined by the runtime (.Net Core 3.1), memory (512 MB), architecture (x86_64), timeout (10 seconds), etc.

Every Lambda execution environment has the following lifecycle:
- Init – Execution env creation based on configuration, download function code & layers, runtime initialization and function initialization (code outside function handler, eg: constructor)
- Invoke – Function handler execution
- Shutdown – Triggered when there are no invocations for a period of time; class destruction, runtime shutdown and environment deletion.


The Init phase is where function initialization happens; this includes class instantiation (constructor) and initialization of constants / static / readonly variables, etc.  The class instantiated in Init phase is used for handling subsequent function invocations.

Since the Init phase doesn’t repeat for each function invocation, all dependency resolutions / dependent classes’ instantiation should happen here, i.e. in the constructor of the class having the function handler.  Doing this will improve the execution time of the function handler, since all required dependencies are already instantiated and available in the Invoke phase.

To be able to unit test the function handler code, all dependencies should be injectable.  This can be achieved by having two constructors:
- Default parameter-less constructor 
  - Instantiate all dependent classes in this constructor. This constructor will be used when the lambda execution environment initializes the class having the function handler. 
- Parameterized constructor 
  - Accepts all dependencies. This constructor will be used in unit tests and will accept mock implementations of all dependencies.
  
Both the constructors will initialize all the dependent classes. The parameterized constructor will be used only in unit tests.

## Reference Implementation

This is a reference implementation for a Lambda function that gets geo information for a given IP address (passed a string parameter).

The Lambda function class has the following dependencies:
- IConfiguration – To fetch configuration from appsettings.json
- IMyIPService – To fetch geo information for given IP address from external service
