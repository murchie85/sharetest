$cyberarkPassword = @{
    value = @{
        Provider = "15"
        Parameters = @{
            Folder = "root"
            Object = "bah"
        }
    }
} | ConvertTo-Json -Compress  # Converts to a valid JSON string

$properties["cyberarkPassword"] = $cyberarkPassword  # Assign the valid JSON string
