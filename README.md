Certainly! Below are examples illustrating those principles with respect to `software.amazon.awssdk.services.kinesis`:

**1. One assert per test & Test one thing at a time**

When testing a `PutRecordRequest`, you want to make sure that each test focuses on a single attribute of the request.

```java
@Test
public void testPartitionKeySetting() {
    PutRecordRequest request = PutRecordRequest.builder()
                                               .partitionKey("myPartitionKey")
                                               .build();

    assertEquals("myPartitionKey", request.partitionKey());
}

@Test
public void testDataSetting() {
    SdkBytes data = SdkBytes.fromUtf8String("someData");
    PutRecordRequest request = PutRecordRequest.builder()
                                               .data(data)
                                               .build();

    assertEquals(data, request.data());
}
```

**2. More than one test per unit**

The `KinesisClient` can be mocked to test the behavior of a unit that interacts with it. For instance, let's test a simple producer that pushes data to Kinesis:

```java
public class SimpleProducer {
    private final KinesisClient kinesisClient;

    public SimpleProducer(KinesisClient kinesisClient) {
        this.kinesisClient = kinesisClient;
    }

    public void sendData(String data) {
        kinesisClient.putRecord(PutRecordRequest.builder()
                                                .data(SdkBytes.fromUtf8String(data))
                                                .partitionKey("someKey")
                                                .streamName("myStream")
                                                .build());
    }
}
```

Now, you can write multiple tests for this `SimpleProducer`:

```java
@Test
public void testSendDataCallsPutRecord() {
    KinesisClient mockClient = mock(KinesisClient.class);
    SimpleProducer producer = new SimpleProducer(mockClient);
    
    producer.sendData("testData");

    verify(mockClient).putRecord(any(PutRecordRequest.class));
}

@Test
public void testSendDataUsesCorrectStream() {
    // ...
}
```

**3. No hardcoded values, unless it has specific meaning**

Use constants or parameterized tests:

```java
private static final String TEST_STREAM_NAME = "myStream";

@Test
public void testStreamNameSetting() {
    PutRecordRequest request = PutRecordRequest.builder()
                                               .streamName(TEST_STREAM_NAME)
                                               .build();

    assertEquals(TEST_STREAM_NAME, request.streamName());
}
```

**4. No retention or manipulation of state**

Ensure that tests do not depend on any shared state:

```java
@BeforeEach
public void setup() {
    // Set up fresh state for each test
}
```

**Happy Path Test**:

```java
@Test
public void testPutRecordHappyPath() {
    KinesisClient mockClient = mock(KinesisClient.class);
    when(mockClient.putRecord(any())).thenReturn(PutRecordResponse.builder().sequenceNumber("12345").build());

    SimpleProducer producer = new SimpleProducer(mockClient);
    producer.sendData("testData");

    verify(mockClient).putRecord(any(PutRecordRequest.class));
}
```

**Unhappy Path Test**:

```java
@Test
public void testPutRecordFailure() {
    KinesisClient mockClient = mock(KinesisClient.class);
    when(mockClient.putRecord(any())).thenThrow(KinesisException.builder().message("Error occurred").build());

    SimpleProducer producer = new SimpleProducer(mockClient);

    assertThrows(KinesisException.class, () -> producer.sendData("testData"));
}
```

In these examples, the `mock()` and related methods are from the Mockito framework, which is widely used in the Java ecosystem for creating mock objects in unit tests. Remember to include necessary dependencies when trying to use Mockito or any other testing frameworks.