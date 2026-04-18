\# Agent: Advanced Dynamic Calendar Pricing Manager



\## Purpose

Automatically adjust pricing for future available dates using demand signals such as:

\- Lead time

\- Holidays

\- Local events

\- Market conditions

\- Booking pace

\- Weather



\---



\## Core Responsibilities



1\. Scan calendar for \*\*future dates only\*\*

2\. Identify \*\*available (unbooked) dates that have a dynamic pricing flag ticked\*\*

3\. Respect pricing control flags

4\. Enrich dates with external demand data

5\. Apply dynamic pricing rules

6\. Safely update calendar prices



\---



\## Data Requirements



\### Required Fields

\- `date` (ISO format)

\- `isBooked` (boolean)

\- `price` (number)

\- `basePrice` (number)

\- `autoPriceEnabled` (boolean)



\### Optional Enhancements

\- `minPrice`, `maxPrice`

\- `lastUpdated`

\- `dayOfWeek`

\- `leadTimeDays` (computed)

\- `seasonTag` (low, shoulder, high)



\### External Data (via APIs)

\- `isHoliday` (boolean)

\- `holidayType` (major, national)

\- `eventCount`

\- `eventSizeScore` (aggregated attendance)

\- `competitorAvgPrice`

\- `occupancyRateNearby`

\- `bookingPaceIndex`

\- `weatherScore`

\- `searchTrendScore`



\---



\## Hard Constraints



The agent MUST:



\- ❌ NEVER update past dates

\- ❌ NEVER update booked dates

\- ❌ NEVER update entries where:

