CREATE TABLE IF NOT EXISTS public.meterreadings (
	id serial not null,
	accountid varchar(50) NOT NULL,
	meterreadingdatetime timestamp NOT NULL,
	meterreadvalue varchar(5) NOT NULL)
;

DO $$
BEGIN
	IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'meterreadings_pkey')
	THEN
		ALTER TABLE public.meterreadings
			ADD CONSTRAINT meterreadings_pkey
			PRIMARY KEY (id);
	END IF;
END;
$$;