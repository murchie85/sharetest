```java
import io.awspring.cloud.sqs.listener.Message;
import org.json.JSONObject;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import static org.mockito.Mockito.*;

public class SQSMessageListenerTest {

    @Mock
    private DataRouterService routerService;

    @InjectMocks
    private SQSMessageListener<Object> sqsMessageListener;

    @BeforeEach
    public void setUp() {
        MockitoAnnotations.openMocks(this);
    }

    @Test
    public void testOnMessage_successfulProcessing() {
        Message<Object> message = mock(Message.class);
        JSONObject jsonObject = new JSONObject("{\"key\":\"value\"}");
        when(message.getPayload()).thenReturn(jsonObject);

        sqsMessageListener.onMessage(message);

        verify(routerService, times(1)).processS3Object(jsonObject);
    }

    @Test
    public void testOnMessage_withException() {
        Message<Object> message = mock(Message.class);
        JSONObject jsonObject = new JSONObject("{\"key\":\"value\"}");
        when(message.getPayload()).thenReturn(jsonObject);
        doThrow(new RuntimeException()).when(routerService).processS3Object(jsonObject);

        sqsMessageListener.onMessage(message);

        verify(routerService, times(1)).processS3Object(jsonObject);
        // Additional assertions or verifications can be made here to ensure correct behavior upon exception.
    }
}

```

```java
import org.junit.jupiter.api.Assertions;

@Test
public void testOnMessage_withException() {
    Message<Object> message = mock(Message.class);
    
    JSONObject jsonObject = null;
    try {
        jsonObject = new JSONObject("{\"key\":\"value\"}");
    } catch (Exception e) {
        fail("Failed to create JSONObject");
    }

    // Asserting that jsonObject is not null
    Assertions.assertNotNull(jsonObject, "JSONObject should not be null");
    
    when(message.getPayload()).thenReturn(jsonObject);
    
    doAnswer(invocation -> {
        throw new Exception("Mock exception");
    }).when(routerService).processS3Object(jsonObject);

    sqsMessageListener.onMessage(message);

    try {
        verify(routerService, times(1)).processS3Object(jsonObject);
    } catch (Exception e) {
        // This block will be executed if the exception is thrown.
        // You can optionally assert something here if needed.
    }

    // Additional assertions or verifications can be made here.
}

```