'use client';

import { useEffect } from 'react';

export default function Error({
    error,
    reset,
}: {
    error: Error & { digest?: string };
    reset: () => void;
}) {
    useEffect(() => {
        console.error(error);
    }, [error]);

    return (
        <section className="reviews primary-bg empty-space min-h-[50vh] flex items-center justify-center">
            <div className="container text-center">
                <h2 className="title-text white-color mb-6">Something went wrong!</h2>
                <p className="text-white mb-8 opacity-80">
                    We were unable to load the reviews. Please try again later.
                </p>
                <button
                    onClick={() => reset()}
                    className="button bg-white text-primary-bg px-6 py-2 rounded hover:bg-gray-100 transition-colors"
                >
                    Try again
                </button>
            </div>
        </section>
    );
}
