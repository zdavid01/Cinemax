export interface Movie {
    id: string;
    title: string;
    length: number;
    genre: string;
    director: string;
    actors: string;
    description: string;
    imageUrl: string;
    trailerLink: string;
    rating: string;
    price?: number;
}
