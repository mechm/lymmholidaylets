
// Use redirect from next/navigation for server-side redirection
import { redirect } from 'next/navigation';

// Define the Props type for the dynamic segment [id]
interface CalendarPageProps {
  params: {
    id: string; // The value of the ID
  };
}

// Server Component (must be async)
export default async function CalendarDownloadPage({ params }: CalendarPageProps) {
  const { id } = params;
  
  // 1. Construct the target URL (the internal API route)
  const downloadUrl = `/api/ical/${id}`;
  
  // 2. Issue the redirect
  // When the browser hits /calendar/1, it immediately receives a 307 redirect 
  // to /api/ical/1, which then serves the file with the necessary headers.
  redirect(downloadUrl);
  
  // NOTE: This component will never render any JSX, but for completeness, 
  // you might return null or an empty fragment if needed, though redirect 
  // halts execution.
}

// import { Key, ReactElement, JSXElementConstructor, ReactNode, ReactPortal } from "react"

// // Tells Next.js to render this page dynamically on every request (SSR behavior)
// // This is the simplest way to prevent fetching at build time.
// export const dynamic = 'force-dynamic';

// // ----------------------------------------------------------------------
// // UPDATED: Use an environment variable with a safe local fallback.
// // Fallback port (8080) should match your local .NET API port when not in Docker.
// //const API_BASE_URL = 'http://localhost:5026';
// const API_BASE_URL = process.env.API_BASE_URL || 'http://localhost:8080';
// const FULL_API_URL = `${API_BASE_URL}/api/calendar/ical/1.ics?s=D93DA743-F66D-4D8E-97E1-6F94D81E0ABE`;


// // ----------------------------------------------------------------------

// //async function getWeatherData() {
// export async function GET(){
//     try {
//         // This fetch runs *at runtime* when a user requests the page
//         // const response = await fetch(FULL_API_URL, {
//         //     // Ensure no caching is performed, reinforcing the dynamic behavior
//         //     cache: 'no-store',
//         // });

//         // 1. Fetch the iCal data from the external API
//         const response = await fetch(FULL_API_URL, {
//             headers: {
//                 'Accept': 'text/calendar',
//             },
//         });

//         if (!response.ok) {
//             // Handle non-200 responses from the external server
//             return new Response(`Failed to fetch iCal: ${response.statusText}`, {
//                 status: response.status,
//             });
//        }

// // 3. Define the crucial headers for the browser download
//     const filename = `${1}.ics`;
//     const headers = {
//       // Set the content type explicitly to iCalendar format
//       'Content-Type': 'text/calendar; charset=utf-8', 
      
//       // *** THIS HEADER TRIGGERS THE DOWNLOAD/CALENDAR OPENER ***
//       // 'attachment' forces the browser to download the file.
//       'Content-Disposition': `attachment; filename="${filename}"`, 
//     };

//     // 4. Return the external response's body (the iCal bytes) with the new headers.
//     // The browser will receive the content and initiate the download.
//     return new Response(response.body, {
//       status: 200,
//       headers: headers,
//     });

//   } catch (error) {
//     console.error('Error serving iCal file:', error);
//     return new Response('Internal Server Error while proxying iCal feed.', { status: 500 });
//   }

//         // if (!response.ok) {
//         //     // This will handle the case where the API is down during a request
//         //     throw new Error(`API call failed with status: ${response.status}`);
//         // }

//     //     return response.json();
//     // } catch (error) {
//     //     console.error('Failed to fetch weather data:', error);
//     //     return null; // Handle failure gracefully
//     // }
// }

// // export default async function Page() {
// //     const data = await getWeatherData();

// //     // return (
// //     //     <main>
// //     //         <h1>Lymm Holiday Lets - Weather Forecast</h1>
// //     //         {data ? (
// //     //             <ul>
// //     //                 {data.map((item: { date: string; summary: string; temperatureC: string; }) => (
// //     //                     <li>
// //     //                         {item.date} - {item.summary} ({item.temperatureC}�C)
// //     //                     </li>
// //     //                 ))}
// //     //             </ul>
// //     //         ) : (
// //     //             <p>Loading weather data failed. Please try again later.</p>
// //     //         )}
// //     //     </main>
// //     // );
// // }