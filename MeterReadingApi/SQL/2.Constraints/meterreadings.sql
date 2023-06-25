DO $$
BEGIN
	IF NOT EXISTS (select 1 from pg_constraint c join pg_namespace n on n.oid = c.connamespace where c.conname = 'fk_meterreadings_customer' and n.nspname = 'public')
	THEN
		ALTER TABLE public.meterreadings
			ADD CONSTRAINT fk_meterreadings_customer
			FOREIGN KEY (accountid) REFERENCES public.customer (accountid);
	END IF;
END;
$$;