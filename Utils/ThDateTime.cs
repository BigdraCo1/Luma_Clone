namespace alma.Utils;

public class ThDateTime {
    public static DateTime Now() {
        return DateTime.UtcNow.AddHours(7);
    }
}