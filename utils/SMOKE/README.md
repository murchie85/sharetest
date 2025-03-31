# Smoke format

```java
@Tag("smoke")
@DisplayName("SMOKE 1: Ensure object is downloaded and unpacked")
@Test
void testDownloadAndUnpack() {
    // Test logic here
}

@Tag("smoke")
@DisplayName("SMOKE 2: Ensure full message sent to kinesis")
@Test
void testSendToKinesis() {
    // Test logic here
}

// Add more tests...

```


## POM SETTINGS Capitalising on Maven Verify phase

```xml

<build>
    <plugins>
        <plugin>
            <groupId>org.apache.maven.plugins</groupId>
            <artifactId>maven-surefire-plugin</artifactId>
            <version>3.1.2</version>
            <configuration>
                <groups>smoke</groups>
                <excludedGroups>slow</excludedGroups>
            </configuration>
        </plugin>
    </plugins>
</build>



<build>
    <plugins>
        <!-- Other plugins... -->
        <plugin>
            <groupId>org.apache.maven.plugins</groupId>
            <artifactId>maven-failsafe-plugin</artifactId>
            <version>3.0.0-M7</version>
            <executions>
                <execution>
                    <id>integration-tests</id>
                    <goals>
                        <goal>integration-test</goal>
                        <goal>verify</goal>
                    </goals>
                    <configuration>
                        <includes>
                            <include>**/Smoke*.java</include>
                        </includes>
                    </configuration>
                </execution>
            </executions>
        </plugin>
    </plugins>
</build>
```