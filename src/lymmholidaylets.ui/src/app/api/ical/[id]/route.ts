import { NextRequest } from 'next/server';

// NOTE: Replace this with your actual external C# API URL structure
//const EXTERNAL_BASE_URL = 'https://your-external-api.com/api/calendar/get-ics';

const API_BASE_URL = process.env.API_BASE_URL || 'http://localhost:8080';
const FULL_API_URL = `${API_BASE_URL}/api/calendar/ical/1.ics?s=D93DA743-F66D-4D8E-97E1-6F94D81E0ABE`;

// Define the Props type for the dynamic segment [id]
interface RouteParams {
  params: {
    id: string;
  };
}

export async function GET(request: NextRequest, { params }: RouteParams) {
  const { id } = params;
  
  // 1. Construct the URL for the external C# API
 // const externalUrl = `${EXTERNAL_BASE_URL}/${id}`;
  const externalUrl = FULL_API_URL;
  
  try {
    // 2. Fetch the data from the external API
    const externalResponse = await fetch(externalUrl);

    if (!externalResponse.ok) {
      // Handle non-200 responses from the external server
      return new Response(`Failed to fetch iCal: ${externalResponse.statusText}`, {
        status: externalResponse.status,
      });
    }

    // 3. Define the crucial headers for the browser download
    const filename = `${id}.ics`;
    const headers = {
      // Set the content type explicitly to iCalendar format
      'Content-Type': 'text/calendar; charset=utf-8', 
      
      // *** THIS HEADER TRIGGERS THE DOWNLOAD/CALENDAR OPENER ***
      // 'attachment' forces the browser to download the file.
      'Content-Disposition': `attachment; filename="${filename}"`, 
    };

    // 4. Return the external response's body (the iCal bytes) with the new headers.
    // The browser will receive the content and initiate the download.
    return new Response(externalResponse.body, {
      status: 200,
      headers: headers,
    });

  } catch (error) {
    console.error('Error serving iCal file:', error);
    return new Response('Internal Server Error while proxying iCal feed.', { status: 500 });
  }
}