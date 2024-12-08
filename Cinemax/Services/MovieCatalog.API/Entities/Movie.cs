﻿namespace MovieCatalog.API.Entities;

public class Movie
{
    public string Id { get; set; }
    public string Title { get; set; }
    public int Length { get; set; }
    public string Genre { get; set; }
    public string Director { get; set; }
    public string Actors { get; set; }
    public string Description { get; set; }
}