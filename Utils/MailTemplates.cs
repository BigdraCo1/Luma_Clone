namespace alma.Utils;

public class MailTemplates {
    public const string accepted = """
<!DOCTYPE html>
<html lang="th">
    <head>
        <meta charset="UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>Registration accepted</title>
    </head>
    <body
        style="
            margin: 0;
            padding: 0;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
        ">
        <table role="presentation" style="width: 100%; border-collapse: collapse; padding: 20px">
            <tr>
                <td align="center">
                    <table
                        role="presentation"
                        style="
                            max-width: 600px;
                            border-collapse: collapse;
                            background-color: white;
                            border-radius: 20px;
                            overflow: hidden;
                            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
                        ">
                        <tr>
                            <td style="padding: 20px; background-color: #000480; color: white">
                                <h1
                                    style="
                                        margin: 0;
                                        font-size: 20px;
                                        font-weight: 500;
                                        color: #78da4b;
                                    ">
                                    <svg
                                        xmlns="http://www.w3.org/2000/svg"
                                        width="20"
                                        height="20"
                                        viewBox="0 0 24 24"
                                        fill="none"
                                        stroke="#78da4b"
                                        stroke-width="3"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                        class="lucide lucide-circle-check-big">
                                        <path d="M21.801 10A10 10 0 1 1 17 3.335" />
                                        <path d="m9 11 3 3L22 4" />
                                    </svg>
                                    {accepted}
                                </h1>
                                <h2 style="margin: 10px 0 0; font-size: 28px; font-weight: 700">
                                    {name}
                                </h2>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 20px; border-bottom: 1px solid #eee">
                                <table
                                    role="presentation"
                                    style="width: 100%; border-collapse: collapse">
                                    <tr>
                                        <td style="padding: 10px">
                                            <div
                                                style="
                                                    margin: 0;
                                                    color: #666;
                                                    font-size: 16px;
                                                    display: flex;
                                                    align-items: center;
                                                    flex-direction: row;
                                                    gap: 10px;
                                                ">
                                                <svg
                                                    xmlns="http://www.w3.org/2000/svg"
                                                    width="38"
                                                    height="38"
                                                    viewBox="0 0 24 24"
                                                    fill="none"
                                                    stroke="currentColor"
                                                    stroke-width="2"
                                                    stroke-linecap="round"
                                                    stroke-linejoin="round"
                                                    class="lucide lucide-calendar-check-2">
                                                    <path d="M8 2v4" />
                                                    <path d="M16 2v4" />
                                                    <path
                                                        d="M21 14V6a2 2 0 0 0-2-2H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h8" />
                                                    <path d="M3 10h18" />
                                                    <path d="m16 20 2 2 4-4" />
                                                </svg>
                                                <div class="time-detail">
                                                    <strong style="color: #333">
                                                        {startAtDate}
                                                    </strong>
                                                    <br />
                                                    <span>{startAtTime} - {endAtTime}</span>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 10px">
                                            <div
                                                style="
                                                    margin: 0;
                                                    color: #666;
                                                    font-size: 16px;
                                                    display: flex;
                                                    align-items: center;
                                                    flex-direction: row;
                                                    gap: 10px;
                                                ">
                                                <svg
                                                    xmlns="http://www.w3.org/2000/svg"
                                                    width="38"
                                                    height="38"
                                                    viewBox="0 0 24 24"
                                                    fill="none"
                                                    stroke="currentColor"
                                                    stroke-width="2"
                                                    stroke-linecap="round"
                                                    stroke-linejoin="round"
                                                    class="lucide lucide-map-pin">
                                                    <path
                                                        d="M20 10c0 4.993-5.539 10.193-7.399 11.799a1 1 0 0 1-1.202 0C9.539 20.193 4 14.993 4 10a8 8 0 0 1 16 0" />
                                                    <circle cx="12" cy="10" r="3" />
                                                </svg>
                                                <div class="time-detail">
                                                    <strong style="color: #333">
                                                        <span>{locationTitle}</span>
                                                    </strong>
                                                    <br />
                                                    <span>{locationSubtitle}</span>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 20px">
                                <p
                                    style="
                                        margin: 0 0 20px;
                                        color: #333;
                                        font-size: 16px;
                                        line-height: 1.6;
                                    ">
                                    {welcomeMessage}
                                </p>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 0 20px 30px; text-align: center">
                                <a
                                    href="{eventUrl}"
                                    style="
                                        display: inline-block;
                                        padding: 14px 30px;
                                        background-color: #000480;
                                        color: white;
                                        text-decoration: none;
                                        font-weight: 500;
                                        border-radius: 6px;
                                        font-size: 16px;
                                    ">
                                    {eventPage}
                                </a>
                            </td>
                        </tr>
                        <tr>
                            <td
                                style="
                                    padding: 10px 30px;
                                    text-align: center;
                                    border-top: 1px solid #eee;
                                ">
                                <p style="margin: 0; color: #666; font-size: 14px">
                                    &copy; 2025 Alma. All Rights Reserved.
                                </p>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </body>
</html>
""";

    public const string rejected = """
<!DOCTYPE html>
<html lang="th">
    <head>
        <meta charset="UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>Registration rejected</title>
    </head>
    <body
        style="
            margin: 0;
            padding: 0;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
        ">
        <table role="presentation" style="width: 100%; border-collapse: collapse; padding: 20px">
            <tr>
                <td align="center">
                    <table
                        role="presentation"
                        style="
                            max-width: 600px;
                            border-collapse: collapse;
                            background-color: white;
                            border-radius: 20px;
                            overflow: hidden;
                            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
                        ">
                        <tr>
                            <td style="padding: 20px; background-color: #000480; color: white">
                                <h1
                                    style="
                                        margin: 0;
                                        font-size: 20px;
                                        font-weight: 500;
                                        color: #df3b3b;
                                    ">
                                    <svg
                                        xmlns="http://www.w3.org/2000/svg"
                                        width="24"
                                        height="24"
                                        viewBox="0 0 24 24"
                                        fill="none"
                                        stroke="#df3b3b"
                                        stroke-width="2"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                        class="lucide lucide-circle-x">
                                        <circle cx="12" cy="12" r="10" />
                                        <path d="m15 9-6 6" />
                                        <path d="m9 9 6 6" />
                                    </svg>
                                    {rejected}
                                </h1>
                                <h2 style="margin: 10px 0 0; font-size: 28px; font-weight: 700">
                                    {name}
                                </h2>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 20px">
                                <p
                                    style="
                                        margin: 0 0 20px;
                                        color: #333;
                                        font-size: 16px;
                                        line-height: 1.6;
                                    ">
                                    {rejectMessage}
                                </p>
                            </td>
                        </tr>
                        <tr>
                            <td
                                style="
                                    padding: 10px 30px;
                                    text-align: center;
                                    border-top: 1px solid #eee;
                                ">
                                <p style="margin: 0; color: #666; font-size: 14px">
                                    &copy; 2025 Alma. All Rights Reserved.
                                </p>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </body>
</html>
""";

    public const string subscription = """
<!DOCTYPE html>
<html lang="th">
    <head>
        <meta charset="UTF-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>New event</title>
    </head>
    <body
        style="
            margin: 0;
            padding: 0;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
        ">
        <table role="presentation" style="width: 100%; border-collapse: collapse; padding: 20px">
            <tr>
                <td align="center">
                    <table
                        role="presentation"
                        style="
                            max-width: 600px;
                            border-collapse: collapse;
                            background-color: white;
                            border-radius: 20px;
                            overflow: hidden;
                            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
                        ">
                        <!-- Header -->
                        <tr>
                            <td style="padding: 20px; background-color: #000480; color: white">
                                <h1 style="margin: 0; font-size: 20px; font-weight: 500">
                                    {newEvent}
                                </h1>
                                <p style="margin: 0; color: #a3a3a3">{by} {from}</p>
                                <h2 style="margin: 10px 0 0; font-size: 28px; font-weight: 700">
                                    {name}
                                </h2>
                            </td>
                        </tr>

                        <!-- Date and Location -->
                        <tr>
                            <td style="padding: 20px">
                                <table
                                    role="presentation"
                                    style="width: 100%; border-collapse: collapse">
                                    <tr>
                                        <td style="padding: 10px">
                                            <div
                                                style="
                                                    margin: 0;
                                                    color: #666;
                                                    font-size: 16px;
                                                    display: flex;
                                                    align-items: center;
                                                    flex-direction: row;
                                                    gap: 10px;
                                                ">
                                                <svg
                                                    xmlns="http://www.w3.org/2000/svg"
                                                    width="38"
                                                    height="38"
                                                    viewBox="0 0 24 24"
                                                    fill="none"
                                                    stroke="currentColor"
                                                    stroke-width="2"
                                                    stroke-linecap="round"
                                                    stroke-linejoin="round"
                                                    class="lucide lucide-calendar-check-2">
                                                    <path d="M8 2v4" />
                                                    <path d="M16 2v4" />
                                                    <path
                                                        d="M21 14V6a2 2 0 0 0-2-2H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h8" />
                                                    <path d="M3 10h18" />
                                                    <path d="m16 20 2 2 4-4" />
                                                </svg>
                                                <div class="time-detail">
                                                    <strong style="color: #333">
                                                        {startAtDate}
                                                    </strong>
                                                    <br />
                                                    <span>{startAtTime} - {endAtTime}</span>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 10px">
                                            <div
                                                style="
                                                    margin: 0;
                                                    color: #666;
                                                    font-size: 16px;
                                                    display: flex;
                                                    align-items: center;
                                                    flex-direction: row;
                                                    gap: 10px;
                                                ">
                                                <svg
                                                    xmlns="http://www.w3.org/2000/svg"
                                                    width="38"
                                                    height="38"
                                                    viewBox="0 0 24 24"
                                                    fill="none"
                                                    stroke="currentColor"
                                                    stroke-width="2"
                                                    stroke-linecap="round"
                                                    stroke-linejoin="round"
                                                    class="lucide lucide-map-pin">
                                                    <path
                                                        d="M20 10c0 4.993-5.539 10.193-7.399 11.799a1 1 0 0 1-1.202 0C9.539 20.193 4 14.993 4 10a8 8 0 0 1 16 0" />
                                                    <circle cx="12" cy="10" r="3" />
                                                </svg>
                                                <div class="time-detail">
                                                    <strong style="color: #333">
                                                        {locationTitle}
                                                    </strong>
                                                    <br />
                                                    <span>{locationSubtitle}</span>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>

                        <!-- Event Description -->
                        <tr>
                            <td style="padding: 0 20px 20px">
                                <pre
                                    style="
                                        margin: 0 0 20px;
                                        color: #333;
                                        font-size: 16px;
                                        line-height: 1.6;
                                        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                                    ">{description}</pre
                                >
                            </td>
                        </tr>

                        <!-- CTA Button -->
                        <tr>
                            <td style="padding: 0 20px 30px; text-align: center">
                                <a
                                    href="{eventUrl}"
                                    style="
                                        display: inline-block;
                                        padding: 14px 30px;
                                        background-color: #000480;
                                        color: white;
                                        text-decoration: none;
                                        font-weight: 500;
                                        border-radius: 6px;
                                        font-size: 16px;
                                    ">
                                    {viewEvent}
                                </a>
                            </td>
                        </tr>

                        <!-- Footer -->
                        <tr>
                            <td
                                style="
                                    padding: 10px 30px;
                                    text-align: center;
                                    border-top: 1px solid #eee;
                                ">
                                <p style="margin: 0; color: #666; font-size: 14px">
                                    &copy; 2025 Alma. All Rights Reserved.
                                </p>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </body>
</html>
""";
}