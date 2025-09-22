-- Run this SQL in Supabase SQL editor to create the necessary tables

CREATE TABLE IF NOT EXISTS public.users (
  id uuid PRIMARY KEY,
  email text UNIQUE NOT NULL,
  passwordhash text NOT NULL,
  fullname text,
  createdat timestamptz DEFAULT now()
);

CREATE TABLE IF NOT EXISTS public.stock (
  id uuid PRIMARY KEY,
  name text NOT NULL,
  quantity integer NOT NULL DEFAULT 0,
  updatedat timestamptz DEFAULT now()
);

-- recommended: configure Row Level Security (RLS) on Supabase for public REST if needed