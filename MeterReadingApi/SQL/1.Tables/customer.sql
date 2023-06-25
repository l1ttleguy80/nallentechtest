CREATE TABLE IF NOT EXISTS public.customer (
	accountid varchar(50) NOT NULL,
	firstname varchar(100) NOT NULL,
	lastname varchar(100) NOT NULL)
;

DO $$
BEGIN
	IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'customer_pkey')
	THEN
		ALTER TABLE public.customer
			ADD CONSTRAINT customer_pkey
			PRIMARY KEY (accountid);
	END IF;
END;
$$;