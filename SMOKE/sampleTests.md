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
ArgumentCaptor<JSONObject> argument = ArgumentCaptor.forClass(JSONObject.class);
verify(routerService, times(1)).processS3Object(argument.capture());
JSONObject actualJsonObject = argument.getValue();
// Now, assert against the actualJsonObject or print it out to see the content.

```


```java
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import software.amazon.awssdk.services.sts.model.GetCallerIdentityResponse;

import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class AWSAuthenticationTest {

    @InjectMocks
    private AWSAuthentication awsAuthentication;

    @Mock
    private AwsAuthProperties authProperties;

    @Mock
    private AWSConfiguration awsHttpClientConfig;

    @Mock
    private StsClient stsClient;

    @BeforeEach
    void setUp() {
        when(awsAuthentication.getStsClient()).thenReturn(stsClient);
    }

    @Test
    void testInit() {
        // Given
        Map<String, String> coreAccounts = new HashMap<>();
        coreAccounts.put("testAccount", "testName");
        when(authProperties.getCoreAccounts()).thenReturn(coreAccounts);
        when(stsClient.getCallerIdentity()).thenReturn(GetCallerIdentityResponse.builder().arn("testArn").build());

        // When
        awsAuthentication.init();

        // Then
        verify(authProperties, times(1)).getCoreAccounts();
        verify(stsClient, times(1)).getCallerIdentity();
    }
    
    // Add more tests for other methods
}

```