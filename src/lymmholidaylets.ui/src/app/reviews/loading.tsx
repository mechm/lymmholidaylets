export default function Loading() {
    return (
        <section className="reviews primary-bg empty-space">
            <div className="container">
                <div className="title-box text-center mb-10">
                    <h2 className="title-text white-color">Reviews</h2>
                </div>

                <div className="row">
                    <div className="col-md-9 mx-auto">
                        <div className="reviews-grid grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                            {[1, 2, 3].map((item) => (
                                <div key={item} className="reviews-box mb-8 animate-pulse bg-white/5 rounded-lg p-6">
                                    <div className="reviews-info text-center">
                                        <div className="h-8 w-8 bg-white/20 rounded-full mx-auto mb-4"></div>
                                        <div className="h-4 bg-white/20 rounded w-3/4 mx-auto mb-2"></div>
                                        <div className="h-4 bg-white/20 rounded w-1/2 mx-auto"></div>
                                    </div>
                                    <div className="reviews-details text-center mt-4">
                                        <div className="h-4 bg-white/20 rounded w-1/3 mx-auto mb-2"></div>
                                        <div className="h-3 bg-white/20 rounded w-1/4 mx-auto"></div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
}
