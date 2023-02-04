public struct SecretCode {

    static string[] privateCodes = {
        "4-ya8wihZEKqT30OVPlvHgik53WuWL_ES27oUJbU-1vg",
        "9RnZFb_GaUG_xry9BTHZJAanreCbsL5UWzNNkmI9t3qA",
        "zDkHu8vtPUGvGGsNidxUXQ3k02NHq2N0OXce8UEk5oQQ",
        "iS-5Qf3PDkqVCV6nt3uz1wLcrM069P1EehvlCzV2Os1Q",
        "bleeYFG9xUCxA-BmsXIf8wnEO-pFcQPEOhWuJUjVH4yg",
        "D5eIiDGXDUeILAU7eGUBSQaoIcIvKRbUeRQ27_ISfTaw",
        "e7GzeKNdL0qAWCTSzjaIzwLOoXH6W4GUyC31aBGpE9Dw"
    };

    static string[] publicCodes = {
        "63de77f38f40bb08f4c7a40a",
        "63de7ade8f40bb08f4c7b43f",
        "63de7af08f40bb08f4c7b45e",
        "63de7afc8f40bb08f4c7b47c",
        "63de7b068f40bb08f4c7b49c",
        "63de7b1a8f40bb08f4c7b4ea",
        "63de7b268f40bb08f4c7b522"
    };

    public static string Public (int level) => publicCodes[level - 1];
    public static string Private (int level) => privateCodes[level - 1];
}
