--
-- PostgreSQL database dump
--

-- Dumped from database version 16.0
-- Dumped by pg_dump version 16.0

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: public; Type: SCHEMA; Schema: -; Owner: postgres
--

-- *not* creating schema, since initdb creates it


ALTER SCHEMA public OWNER TO postgres;

--
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: postgres
--

COMMENT ON SCHEMA public IS '';


--
-- Name: get_account_created(integer, date, date); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_account_created(select_role integer, from_date date, to_date date) RETURNS TABLE(type_date_year numeric, type_date_month numeric, type_date_day numeric, count_account bigint)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    WITH account_role AS (
        SELECT 
            acc.id, acc.create_date 
        FROM 
            account acc 
        WHERE
            acc.role_id = select_role
    )
    SELECT  
		EXTRACT(YEAR FROM acc.create_date) AS date_year,
		EXTRACT(MONTH FROM acc.create_date) AS date_month,
		EXTRACT(DAY FROM acc.create_date) AS date_day,
        count(*) as count_account
    FROM 
        account_role acc
    WHERE 
        acc.create_date BETWEEN from_date AND (to_date::timestamp + interval '1 day' - interval '1 second')
    GROUP BY 
        date_year,
		date_month,
		date_day
	ORDER BY 
		date_year,
		date_month,
		date_day;
END;
$$;


ALTER FUNCTION public.get_account_created(select_role integer, from_date date, to_date date) OWNER TO postgres;

--
-- Name: get_profit(date, date); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_profit(from_date date, to_date date) RETURNS TABLE(type_date_year numeric, type_date_month numeric, type_date_day numeric, deposit_amount money, refund_amount money, profit money)
    LANGUAGE plpgsql
    AS $$
begin
	Return query (
		with valid_transaction as (
			Select vt.payment_date, vt.payment_amount,
				vt.type_transaction
			from 
				transaction_history vt
			where
				vt.payment_date between from_date and (to_date::timestamp + interval '1 day' - interval '1 second')
		)
		
		Select 
			extract(YEAR FROM vt.payment_date) as year_payment,
			extract(MONTH FROM vt.payment_date) as month_payment,
			extract(DAY FROM vt.payment_date) as day_payment,
			sum(vt.payment_amount * case when vt.type_transaction then 1 else 0 end) as deposit_fee,
    		sum(vt.payment_amount * case when not vt.type_transaction then 1 else 0 end) as refund_fee,
			sum(vt.payment_amount * case when vt.type_transaction then 1 else 0 end) - sum(vt.payment_amount * case when not vt.type_transaction then 1 else 0 end) as profit
		from 
			valid_transaction vt
		group by
			year_payment,
			month_payment,
			day_payment
		order by
			year_payment,
			month_payment,
			day_payment
	);
end;
$$;


ALTER FUNCTION public.get_profit(from_date date, to_date date) OWNER TO postgres;

--
-- Name: get_requests(date, date); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_requests(from_date date, to_date date) RETURNS TABLE(type_date_year numeric, type_date_month numeric, type_date_day numeric, count_request bigint)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT  
		EXTRACT(YEAR FROM r.create_date) AS type_date_year,
		EXTRACT(MONTH FROM r.create_date) AS type_date_month,
		EXTRACT(DAY FROM r.create_date) AS type_date_day,
		count(*) as request_count
    FROM 
        request_tutor_form r
    WHERE 
        r.create_date BETWEEN from_date AND (to_date::timestamp + interval '1 day' - interval '1 second')
    GROUP BY 
        type_date_year,
		type_date_month,
		type_date_day
	ORDER BY 
		type_date_year,
		type_date_month,
		type_date_day;
END;
$$;


ALTER FUNCTION public.get_requests(from_date date, to_date date) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: account; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.account (
    id integer NOT NULL,
    email character varying(100) NOT NULL,
    phone character varying(20) NOT NULL,
    password_hash character varying(255) NOT NULL,
    lock_enable boolean DEFAULT false NOT NULL,
    avatar character varying(255) NOT NULL,
    create_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    role_id integer NOT NULL
);


ALTER TABLE public.account OWNER TO postgres;

--
-- Name: account_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.account_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.account_id_seq OWNER TO postgres;

--
-- Name: account_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.account_id_seq OWNED BY public.account.id;


--
-- Name: customer; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.customer (
    id integer NOT NULL,
    full_name character varying(100) NOT NULL,
    birth date NOT NULL,
    gender character varying(1) NOT NULL,
    address_detail character varying(100) NOT NULL,
    district_id integer NOT NULL,
    account_id integer NOT NULL,
    identity_id integer NOT NULL,
    CONSTRAINT customer_gender_check CHECK (((gender)::text = ANY (ARRAY[('M'::character varying)::text, ('F'::character varying)::text])))
);


ALTER TABLE public.customer OWNER TO postgres;

--
-- Name: customer_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.customer_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.customer_id_seq OWNER TO postgres;

--
-- Name: customer_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.customer_id_seq OWNED BY public.customer.id;


--
-- Name: district; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.district (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    province_id integer NOT NULL
);


ALTER TABLE public.district OWNER TO postgres;

--
-- Name: district_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.district_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.district_id_seq OWNER TO postgres;

--
-- Name: district_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.district_id_seq OWNED BY public.district.id;


--
-- Name: employee; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.employee (
    id integer NOT NULL,
    full_name character varying(100) NOT NULL,
    birth date NOT NULL,
    gender character varying(1) NOT NULL,
    address_detail character varying(100) NOT NULL,
    district_id integer NOT NULL,
    account_id integer NOT NULL,
    identity_id integer NOT NULL,
    CONSTRAINT employee_gender_check CHECK (((gender)::text = ANY (ARRAY[('M'::character varying)::text, ('F'::character varying)::text])))
);


ALTER TABLE public.employee OWNER TO postgres;

--
-- Name: employee_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.employee_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.employee_id_seq OWNER TO postgres;

--
-- Name: employee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employee_id_seq OWNED BY public.employee.id;


--
-- Name: grade; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.grade (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    value integer NOT NULL,
    fee money DEFAULT 0 NOT NULL
);


ALTER TABLE public.grade OWNER TO postgres;

--
-- Name: grade_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.grade_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.grade_id_seq OWNER TO postgres;

--
-- Name: grade_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.grade_id_seq OWNED BY public.grade.id;


--
-- Name: identity_card; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.identity_card (
    id integer NOT NULL,
    identity_number character varying(20) NOT NULL,
    front_identity_card character varying(255) NOT NULL,
    back_identity_card character varying(255) NOT NULL
);


ALTER TABLE public.identity_card OWNER TO postgres;

--
-- Name: identitycard_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.identitycard_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.identitycard_id_seq OWNER TO postgres;

--
-- Name: identitycard_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.identitycard_id_seq OWNED BY public.identity_card.id;


--
-- Name: province; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.province (
    id integer NOT NULL,
    name character varying(50) NOT NULL
);


ALTER TABLE public.province OWNER TO postgres;

--
-- Name: province_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.province_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.province_id_seq OWNER TO postgres;

--
-- Name: province_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.province_id_seq OWNED BY public.province.id;


--
-- Name: tutor_status_detail; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_status_detail (
    id integer NOT NULL,
    context text NOT NULL,
    create_date timestamp without time zone DEFAULT CURRENT_DATE NOT NULL,
    tutor_id integer NOT NULL,
    status_id integer NOT NULL
);


ALTER TABLE public.tutor_status_detail OWNER TO postgres;

--
-- Name: registerstatusdetail_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.registerstatusdetail_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.registerstatusdetail_id_seq OWNER TO postgres;

--
-- Name: registerstatusdetail_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.registerstatusdetail_id_seq OWNED BY public.tutor_status_detail.id;


--
-- Name: request_status; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.request_status (
    id integer NOT NULL,
    context text NOT NULL,
    create_date timestamp without time zone DEFAULT now() NOT NULL,
    tutor_request_id integer NOT NULL
);


ALTER TABLE public.request_status OWNER TO postgres;

--
-- Name: request_tutor_form; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.request_tutor_form (
    id integer NOT NULL,
    students integer NOT NULL,
    additional_detail text,
    create_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    expired_date timestamp without time zone NOT NULL,
    address_detail character varying(255) NOT NULL,
    subject_id integer NOT NULL,
    grade_id integer NOT NULL,
    status_id integer NOT NULL,
    district_id integer NOT NULL,
    customer_id integer NOT NULL,
    CONSTRAINT tutorrequestform_students_check CHECK (((students > 0) AND (students < 6)))
);


ALTER TABLE public.request_tutor_form OWNER TO postgres;

--
-- Name: requeststatus_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.requeststatus_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.requeststatus_id_seq OWNER TO postgres;

--
-- Name: requeststatus_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.requeststatus_id_seq OWNED BY public.request_status.id;


--
-- Name: role; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.role (
    id integer NOT NULL,
    name character varying(20) NOT NULL
);


ALTER TABLE public.role OWNER TO postgres;

--
-- Name: role_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.role_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.role_id_seq OWNER TO postgres;

--
-- Name: role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.role_id_seq OWNED BY public.role.id;


--
-- Name: session_date; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.session_date (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    value integer NOT NULL
);


ALTER TABLE public.session_date OWNER TO postgres;

--
-- Name: sessiondate_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.sessiondate_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.sessiondate_id_seq OWNER TO postgres;

--
-- Name: sessiondate_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.sessiondate_id_seq OWNED BY public.session_date.id;


--
-- Name: status; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.status (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    status_type_id integer NOT NULL,
    vietnamese_name character varying(50)
);


ALTER TABLE public.status OWNER TO postgres;

--
-- Name: status_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.status_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.status_id_seq OWNER TO postgres;

--
-- Name: status_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.status_id_seq OWNED BY public.status.id;


--
-- Name: status_type; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.status_type (
    id integer NOT NULL,
    type character varying(50) NOT NULL
);


ALTER TABLE public.status_type OWNER TO postgres;

--
-- Name: statustype_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.statustype_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.statustype_id_seq OWNER TO postgres;

--
-- Name: statustype_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.statustype_id_seq OWNED BY public.status_type.id;


--
-- Name: subject; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.subject (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    value integer NOT NULL
);


ALTER TABLE public.subject OWNER TO postgres;

--
-- Name: subject_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.subject_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.subject_id_seq OWNER TO postgres;

--
-- Name: subject_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.subject_id_seq OWNED BY public.subject.id;


--
-- Name: transaction_history; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.transaction_history (
    id integer NOT NULL,
    create_date timestamp without time zone DEFAULT CURRENT_DATE NOT NULL,
    payment_date timestamp without time zone,
    payment_amount money NOT NULL,
    context character varying(255),
    employee_id integer NOT NULL,
    tutor_id integer NOT NULL,
    form_id integer NOT NULL,
    type_transaction boolean,
    status_id integer NOT NULL
);


ALTER TABLE public.transaction_history OWNER TO postgres;

--
-- Name: transactionhistory_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.transactionhistory_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.transactionhistory_id_seq OWNER TO postgres;

--
-- Name: transactionhistory_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.transactionhistory_id_seq OWNED BY public.transaction_history.id;


--
-- Name: tutor; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor (
    id integer NOT NULL,
    full_name character varying(100) NOT NULL,
    birth date NOT NULL,
    gender character varying(1) NOT NULL,
    address_detail character varying(255) NOT NULL,
    college character varying(100) NOT NULL,
    area character varying(100) NOT NULL,
    additional_info character varying(255),
    academic_year_from smallint NOT NULL,
    academic_year_to smallint NOT NULL,
    is_active boolean DEFAULT true NOT NULL,
    district_id integer NOT NULL,
    account_id integer NOT NULL,
    identity_id integer NOT NULL,
    tutor_type_id integer NOT NULL,
    status_id integer NOT NULL,
    CONSTRAINT tutor_check CHECK ((academic_year_from < academic_year_to)),
    CONSTRAINT tutor_gender_check CHECK (((gender)::text = ANY (ARRAY[('M'::character varying)::text, ('F'::character varying)::text])))
);


ALTER TABLE public.tutor OWNER TO postgres;

--
-- Name: tutor_apply_form; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_apply_form (
    tutor_request_id integer NOT NULL,
    tutor_id integer NOT NULL,
    enter_date timestamp without time zone DEFAULT CURRENT_DATE,
    status_id integer NOT NULL
);


ALTER TABLE public.tutor_apply_form OWNER TO postgres;

--
-- Name: tutor_grade; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_grade (
    tutor_id integer NOT NULL,
    grade_id integer NOT NULL
);


ALTER TABLE public.tutor_grade OWNER TO postgres;

--
-- Name: tutor_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tutor_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tutor_id_seq OWNER TO postgres;

--
-- Name: tutor_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tutor_id_seq OWNED BY public.tutor.id;


--
-- Name: tutor_request_session; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_request_session (
    tutor_request_id integer NOT NULL,
    session_id integer NOT NULL
);


ALTER TABLE public.tutor_request_session OWNER TO postgres;

--
-- Name: tutor_session; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_session (
    tutor_id integer NOT NULL,
    session_id integer NOT NULL
);


ALTER TABLE public.tutor_session OWNER TO postgres;

--
-- Name: tutor_subject; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_subject (
    tutor_id integer NOT NULL,
    subject_id integer NOT NULL
);


ALTER TABLE public.tutor_subject OWNER TO postgres;

--
-- Name: tutor_teaching_area; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_teaching_area (
    tutor_id integer NOT NULL,
    district_id integer NOT NULL
);


ALTER TABLE public.tutor_teaching_area OWNER TO postgres;

--
-- Name: tutor_type; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_type (
    id integer NOT NULL,
    name character varying(20) NOT NULL,
    value integer NOT NULL
);


ALTER TABLE public.tutor_type OWNER TO postgres;

--
-- Name: tutor_type_tutor_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tutor_type_tutor_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tutor_type_tutor_id_seq OWNER TO postgres;

--
-- Name: tutor_type_tutor_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tutor_type_tutor_id_seq OWNED BY public.tutor_type.id;


--
-- Name: tutorrequestform_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tutorrequestform_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tutorrequestform_id_seq OWNER TO postgres;

--
-- Name: tutorrequestform_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tutorrequestform_id_seq OWNED BY public.request_tutor_form.id;


--
-- Name: account id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account ALTER COLUMN id SET DEFAULT nextval('public.account_id_seq'::regclass);


--
-- Name: customer id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer ALTER COLUMN id SET DEFAULT nextval('public.customer_id_seq'::regclass);


--
-- Name: district id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.district ALTER COLUMN id SET DEFAULT nextval('public.district_id_seq'::regclass);


--
-- Name: employee id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee ALTER COLUMN id SET DEFAULT nextval('public.employee_id_seq'::regclass);


--
-- Name: grade id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grade ALTER COLUMN id SET DEFAULT nextval('public.grade_id_seq'::regclass);


--
-- Name: identity_card id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.identity_card ALTER COLUMN id SET DEFAULT nextval('public.identitycard_id_seq'::regclass);


--
-- Name: province id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.province ALTER COLUMN id SET DEFAULT nextval('public.province_id_seq'::regclass);


--
-- Name: request_status id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_status ALTER COLUMN id SET DEFAULT nextval('public.requeststatus_id_seq'::regclass);


--
-- Name: request_tutor_form id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form ALTER COLUMN id SET DEFAULT nextval('public.tutorrequestform_id_seq'::regclass);


--
-- Name: role id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role ALTER COLUMN id SET DEFAULT nextval('public.role_id_seq'::regclass);


--
-- Name: session_date id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.session_date ALTER COLUMN id SET DEFAULT nextval('public.sessiondate_id_seq'::regclass);


--
-- Name: status id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status ALTER COLUMN id SET DEFAULT nextval('public.status_id_seq'::regclass);


--
-- Name: status_type id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status_type ALTER COLUMN id SET DEFAULT nextval('public.statustype_id_seq'::regclass);


--
-- Name: subject id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subject ALTER COLUMN id SET DEFAULT nextval('public.subject_id_seq'::regclass);


--
-- Name: transaction_history id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history ALTER COLUMN id SET DEFAULT nextval('public.transactionhistory_id_seq'::regclass);


--
-- Name: tutor id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor ALTER COLUMN id SET DEFAULT nextval('public.tutor_id_seq'::regclass);


--
-- Name: tutor_status_detail id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_status_detail ALTER COLUMN id SET DEFAULT nextval('public.registerstatusdetail_id_seq'::regclass);


--
-- Name: tutor_type id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_type ALTER COLUMN id SET DEFAULT nextval('public.tutor_type_tutor_id_seq'::regclass);


--
-- Name: account account_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account
    ADD CONSTRAINT account_email_key UNIQUE (email);


--
-- Name: account account_phone_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account
    ADD CONSTRAINT account_phone_key UNIQUE (phone);


--
-- Name: account account_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account
    ADD CONSTRAINT account_pkey PRIMARY KEY (id);


--
-- Name: customer customer_accountid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_accountid_key UNIQUE (account_id);


--
-- Name: customer customer_identityid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_identityid_key UNIQUE (identity_id);


--
-- Name: customer customer_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_pkey PRIMARY KEY (id);


--
-- Name: district district_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.district
    ADD CONSTRAINT district_name_key UNIQUE (name);


--
-- Name: district district_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.district
    ADD CONSTRAINT district_pkey PRIMARY KEY (id);


--
-- Name: employee employee_accountid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_accountid_key UNIQUE (account_id);


--
-- Name: employee employee_identityid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_identityid_key UNIQUE (identity_id);


--
-- Name: employee employee_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_pkey PRIMARY KEY (id);


--
-- Name: grade grade_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grade
    ADD CONSTRAINT grade_name_key UNIQUE (name);


--
-- Name: grade grade_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grade
    ADD CONSTRAINT grade_pkey PRIMARY KEY (id);


--
-- Name: grade grade_value_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grade
    ADD CONSTRAINT grade_value_key UNIQUE (value);


--
-- Name: identity_card identitycard_identitynumber_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.identity_card
    ADD CONSTRAINT identitycard_identitynumber_key UNIQUE (identity_number);


--
-- Name: identity_card identitycard_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.identity_card
    ADD CONSTRAINT identitycard_pkey PRIMARY KEY (id);


--
-- Name: province province_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.province
    ADD CONSTRAINT province_pkey PRIMARY KEY (id);


--
-- Name: province province_provincename_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.province
    ADD CONSTRAINT province_provincename_key UNIQUE (name);


--
-- Name: tutor_status_detail registerstatusdetail_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_status_detail
    ADD CONSTRAINT registerstatusdetail_pkey PRIMARY KEY (id);


--
-- Name: request_status requeststatus_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_status
    ADD CONSTRAINT requeststatus_pkey PRIMARY KEY (id);


--
-- Name: role role_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role
    ADD CONSTRAINT role_name_key UNIQUE (name);


--
-- Name: role role_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role
    ADD CONSTRAINT role_pkey PRIMARY KEY (id);


--
-- Name: session_date sessiondate_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.session_date
    ADD CONSTRAINT sessiondate_name_key UNIQUE (name);


--
-- Name: session_date sessiondate_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.session_date
    ADD CONSTRAINT sessiondate_pkey PRIMARY KEY (id);


--
-- Name: session_date sessiondate_value_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.session_date
    ADD CONSTRAINT sessiondate_value_key UNIQUE (value);


--
-- Name: status status_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status
    ADD CONSTRAINT status_pkey PRIMARY KEY (id);


--
-- Name: status_type statustype_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status_type
    ADD CONSTRAINT statustype_pkey PRIMARY KEY (id);


--
-- Name: status_type statustype_type_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status_type
    ADD CONSTRAINT statustype_type_key UNIQUE (type);


--
-- Name: subject subject_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subject
    ADD CONSTRAINT subject_name_key UNIQUE (name);


--
-- Name: subject subject_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subject
    ADD CONSTRAINT subject_pkey PRIMARY KEY (id);


--
-- Name: subject subject_value_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subject
    ADD CONSTRAINT subject_value_key UNIQUE (value);


--
-- Name: transaction_history transactionhistory_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history
    ADD CONSTRAINT transactionhistory_pkey PRIMARY KEY (id);


--
-- Name: tutor tutor_accountid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_accountid_key UNIQUE (account_id);


--
-- Name: tutor tutor_identityid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_identityid_key UNIQUE (identity_id);


--
-- Name: tutor tutor_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_pkey PRIMARY KEY (id);


--
-- Name: tutor_type tutor_type_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_type
    ADD CONSTRAINT tutor_type_name_key UNIQUE (name);


--
-- Name: tutor_type tutor_type_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_type
    ADD CONSTRAINT tutor_type_pkey PRIMARY KEY (id);


--
-- Name: tutor_type tutor_type_value_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_type
    ADD CONSTRAINT tutor_type_value_key UNIQUE (value);


--
-- Name: tutor_grade tutorgrade_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_grade
    ADD CONSTRAINT tutorgrade_pkey PRIMARY KEY (tutor_id, grade_id);


--
-- Name: tutor_apply_form tutorqueue_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_apply_form
    ADD CONSTRAINT tutorqueue_pkey PRIMARY KEY (tutor_request_id, tutor_id);


--
-- Name: request_tutor_form tutorrequestform_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_pkey PRIMARY KEY (id);


--
-- Name: tutor_request_session tutorrequestsession_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_request_session
    ADD CONSTRAINT tutorrequestsession_pkey PRIMARY KEY (tutor_request_id, session_id);


--
-- Name: tutor_session tutorsession_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_session
    ADD CONSTRAINT tutorsession_pkey PRIMARY KEY (tutor_id, session_id);


--
-- Name: tutor_subject tutorsubject_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_subject
    ADD CONSTRAINT tutorsubject_pkey PRIMARY KEY (tutor_id, subject_id);


--
-- Name: tutor_teaching_area tutorteachingareas_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_teaching_area
    ADD CONSTRAINT tutorteachingareas_pkey PRIMARY KEY (tutor_id, district_id);


--
-- Name: status uq_name_status_type_id; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status
    ADD CONSTRAINT uq_name_status_type_id UNIQUE (name, status_type_id);


--
-- Name: account account_roleid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account
    ADD CONSTRAINT account_roleid_fkey FOREIGN KEY (role_id) REFERENCES public.role(id);


--
-- Name: customer customer_accountid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_accountid_fkey FOREIGN KEY (account_id) REFERENCES public.account(id);


--
-- Name: customer customer_districtid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_districtid_fkey FOREIGN KEY (district_id) REFERENCES public.district(id);


--
-- Name: customer customer_identityid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_identityid_fkey FOREIGN KEY (identity_id) REFERENCES public.identity_card(id);


--
-- Name: district district_provinceid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.district
    ADD CONSTRAINT district_provinceid_fkey FOREIGN KEY (province_id) REFERENCES public.province(id);


--
-- Name: employee employee_accountid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_accountid_fkey FOREIGN KEY (account_id) REFERENCES public.account(id);


--
-- Name: employee employee_districtid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_districtid_fkey FOREIGN KEY (district_id) REFERENCES public.district(id);


--
-- Name: employee employee_identityid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_identityid_fkey FOREIGN KEY (identity_id) REFERENCES public.identity_card(id);


--
-- Name: tutor fk_tutor_status; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT fk_tutor_status FOREIGN KEY (status_id) REFERENCES public.status(id);


--
-- Name: tutor_status_detail registerstatusdetail_statusid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_status_detail
    ADD CONSTRAINT registerstatusdetail_statusid_fkey FOREIGN KEY (status_id) REFERENCES public.status(id);


--
-- Name: tutor_status_detail registerstatusdetail_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_status_detail
    ADD CONSTRAINT registerstatusdetail_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- Name: request_status requeststatus_tutorrequestid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_status
    ADD CONSTRAINT requeststatus_tutorrequestid_fkey FOREIGN KEY (tutor_request_id) REFERENCES public.request_tutor_form(id);


--
-- Name: status status_statustypeid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status
    ADD CONSTRAINT status_statustypeid_fkey FOREIGN KEY (status_type_id) REFERENCES public.status_type(id);


--
-- Name: transaction_history transaction_history_status_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history
    ADD CONSTRAINT transaction_history_status_id_fkey FOREIGN KEY (status_id) REFERENCES public.status(id);


--
-- Name: transaction_history transaction_history_tutor_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history
    ADD CONSTRAINT transaction_history_tutor_id_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- Name: transaction_history transactionhistory_employeeid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history
    ADD CONSTRAINT transactionhistory_employeeid_fkey FOREIGN KEY (employee_id) REFERENCES public.employee(id);


--
-- Name: transaction_history transactionhistory_formid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history
    ADD CONSTRAINT transactionhistory_formid_fkey FOREIGN KEY (form_id) REFERENCES public.request_tutor_form(id);


--
-- Name: tutor tutor_accountid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_accountid_fkey FOREIGN KEY (account_id) REFERENCES public.account(id);


--
-- Name: tutor tutor_districtid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_districtid_fkey FOREIGN KEY (district_id) REFERENCES public.district(id);


--
-- Name: tutor tutor_identityid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_identityid_fkey FOREIGN KEY (identity_id) REFERENCES public.identity_card(id);


--
-- Name: tutor tutor_tutor_type_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_tutor_type_id_fkey FOREIGN KEY (tutor_type_id) REFERENCES public.tutor_type(id);


--
-- Name: tutor_grade tutorgrade_gradeid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_grade
    ADD CONSTRAINT tutorgrade_gradeid_fkey FOREIGN KEY (grade_id) REFERENCES public.grade(id);


--
-- Name: tutor_grade tutorgrade_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_grade
    ADD CONSTRAINT tutorgrade_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- Name: tutor_apply_form tutorqueue_statusid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_apply_form
    ADD CONSTRAINT tutorqueue_statusid_fkey FOREIGN KEY (status_id) REFERENCES public.status(id);


--
-- Name: tutor_apply_form tutorqueue_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_apply_form
    ADD CONSTRAINT tutorqueue_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- Name: tutor_apply_form tutorqueue_tutorrequestid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_apply_form
    ADD CONSTRAINT tutorqueue_tutorrequestid_fkey FOREIGN KEY (tutor_request_id) REFERENCES public.request_tutor_form(id);


--
-- Name: request_tutor_form tutorrequestform_customerid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_customerid_fkey FOREIGN KEY (customer_id) REFERENCES public.customer(id);


--
-- Name: request_tutor_form tutorrequestform_districtid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_districtid_fkey FOREIGN KEY (district_id) REFERENCES public.district(id);


--
-- Name: request_tutor_form tutorrequestform_gradeid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_gradeid_fkey FOREIGN KEY (grade_id) REFERENCES public.grade(id);


--
-- Name: request_tutor_form tutorrequestform_statusid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_statusid_fkey FOREIGN KEY (status_id) REFERENCES public.status(id);


--
-- Name: request_tutor_form tutorrequestform_subjectid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_subjectid_fkey FOREIGN KEY (subject_id) REFERENCES public.subject(id);


--
-- Name: tutor_request_session tutorrequestsession_sessionid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_request_session
    ADD CONSTRAINT tutorrequestsession_sessionid_fkey FOREIGN KEY (session_id) REFERENCES public.session_date(id);


--
-- Name: tutor_request_session tutorrequestsession_tutorrequestid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_request_session
    ADD CONSTRAINT tutorrequestsession_tutorrequestid_fkey FOREIGN KEY (tutor_request_id) REFERENCES public.request_tutor_form(id);


--
-- Name: tutor_session tutorsession_sessionid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_session
    ADD CONSTRAINT tutorsession_sessionid_fkey FOREIGN KEY (session_id) REFERENCES public.session_date(id);


--
-- Name: tutor_session tutorsession_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_session
    ADD CONSTRAINT tutorsession_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- Name: tutor_subject tutorsubject_subjectid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_subject
    ADD CONSTRAINT tutorsubject_subjectid_fkey FOREIGN KEY (subject_id) REFERENCES public.subject(id);


--
-- Name: tutor_subject tutorsubject_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_subject
    ADD CONSTRAINT tutorsubject_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- Name: tutor_teaching_area tutorteachingareas_districtid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_teaching_area
    ADD CONSTRAINT tutorteachingareas_districtid_fkey FOREIGN KEY (district_id) REFERENCES public.district(id);


--
-- Name: tutor_teaching_area tutorteachingareas_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_teaching_area
    ADD CONSTRAINT tutorteachingareas_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;


--
-- PostgreSQL database dump complete
--


-- TOC entry 5092 (class 0 OID 21314)
-- Dependencies: 233
-- Data for Name: role; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.role (id, name) VALUES (1, 'admin');
INSERT INTO public.role (id, name) VALUES (2, 'employee');
INSERT INTO public.role (id, name) VALUES (3, 'tutor');
INSERT INTO public.role (id, name) VALUES (4, 'customer');

--
-- TOC entry 5086 (class 0 OID 21298)
-- Dependencies: 227
-- Data for Name: province; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.province (id, name) VALUES (1, 'Thành phố Hà Nội');
INSERT INTO public.province (id, name) VALUES (56, 'Tỉnh Khánh Hòa');
INSERT INTO public.province (id, name) VALUES (79, 'Thành phố Hồ Chí Minh');


--
-- TOC entry 5078 (class 0 OID 21279)
-- Dependencies: 219
-- Data for Name: district; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.district (id, name, province_id) VALUES (1, 'Quận Ba Đình', 1);
INSERT INTO public.district (id, name, province_id) VALUES (2, 'Quận Hoàn Kiếm', 1);
INSERT INTO public.district (id, name, province_id) VALUES (3, 'Quận Tây Hồ', 1);
INSERT INTO public.district (id, name, province_id) VALUES (4, 'Quận Long Biên', 1);
INSERT INTO public.district (id, name, province_id) VALUES (5, 'Quận Cầu Giấy', 1);
INSERT INTO public.district (id, name, province_id) VALUES (6, 'Quận Đống Đa', 1);
INSERT INTO public.district (id, name, province_id) VALUES (7, 'Quận Hai Bà Trưng', 1);
INSERT INTO public.district (id, name, province_id) VALUES (8, 'Quận Hoàng Mai', 1);
INSERT INTO public.district (id, name, province_id) VALUES (9, 'Quận Thanh Xuân', 1);
INSERT INTO public.district (id, name, province_id) VALUES (16, 'Huyện Sóc Sơn', 1);
INSERT INTO public.district (id, name, province_id) VALUES (17, 'Huyện Đông Anh', 1);
INSERT INTO public.district (id, name, province_id) VALUES (18, 'Huyện Gia Lâm', 1);
INSERT INTO public.district (id, name, province_id) VALUES (19, 'Quận Nam Từ Liêm', 1);
INSERT INTO public.district (id, name, province_id) VALUES (20, 'Huyện Thanh Trì', 1);
INSERT INTO public.district (id, name, province_id) VALUES (21, 'Quận Bắc Từ Liêm', 1);
INSERT INTO public.district (id, name, province_id) VALUES (250, 'Huyện Mê Linh', 1);
INSERT INTO public.district (id, name, province_id) VALUES (268, 'Quận Hà Đông', 1);
INSERT INTO public.district (id, name, province_id) VALUES (269, 'Thị xã Sơn Tây', 1);
INSERT INTO public.district (id, name, province_id) VALUES (271, 'Huyện Ba Vì', 1);
INSERT INTO public.district (id, name, province_id) VALUES (272, 'Huyện Phúc Thọ', 1);
INSERT INTO public.district (id, name, province_id) VALUES (273, 'Huyện Đan Phượng', 1);
INSERT INTO public.district (id, name, province_id) VALUES (274, 'Huyện Hoài Đức', 1);
INSERT INTO public.district (id, name, province_id) VALUES (275, 'Huyện Quốc Oai', 1);
INSERT INTO public.district (id, name, province_id) VALUES (276, 'Huyện Thạch Thất', 1);
INSERT INTO public.district (id, name, province_id) VALUES (277, 'Huyện Chương Mỹ', 1);
INSERT INTO public.district (id, name, province_id) VALUES (278, 'Huyện Thanh Oai', 1);
INSERT INTO public.district (id, name, province_id) VALUES (279, 'Huyện Thường Tín', 1);
INSERT INTO public.district (id, name, province_id) VALUES (280, 'Huyện Phú Xuyên', 1);
INSERT INTO public.district (id, name, province_id) VALUES (281, 'Huyện Ứng Hòa', 1);
INSERT INTO public.district (id, name, province_id) VALUES (282, 'Huyện Mỹ Đức', 1);
INSERT INTO public.district (id, name, province_id) VALUES (568, 'Thành phố Nha Trang', 56);
INSERT INTO public.district (id, name, province_id) VALUES (569, 'Thành phố Cam Ranh', 56);
INSERT INTO public.district (id, name, province_id) VALUES (570, 'Huyện Cam Lâm', 56);
INSERT INTO public.district (id, name, province_id) VALUES (571, 'Huyện Vạn Ninh', 56);
INSERT INTO public.district (id, name, province_id) VALUES (572, 'Thị xã Ninh Hòa', 56);
INSERT INTO public.district (id, name, province_id) VALUES (573, 'Huyện Khánh Vĩnh', 56);
INSERT INTO public.district (id, name, province_id) VALUES (574, 'Huyện Diên Khánh', 56);
INSERT INTO public.district (id, name, province_id) VALUES (575, 'Huyện Khánh Sơn', 56);
INSERT INTO public.district (id, name, province_id) VALUES (576, 'Huyện Trường Sa', 56);
INSERT INTO public.district (id, name, province_id) VALUES (760, 'Quận 1', 79);
INSERT INTO public.district (id, name, province_id) VALUES (761, 'Quận 12', 79);
INSERT INTO public.district (id, name, province_id) VALUES (764, 'Quận Gò Vấp', 79);
INSERT INTO public.district (id, name, province_id) VALUES (765, 'Quận Bình Thạnh', 79);
INSERT INTO public.district (id, name, province_id) VALUES (766, 'Quận Tân Bình', 79);
INSERT INTO public.district (id, name, province_id) VALUES (767, 'Quận Tân Phú', 79);
INSERT INTO public.district (id, name, province_id) VALUES (768, 'Quận Phú Nhuận', 79);
INSERT INTO public.district (id, name, province_id) VALUES (769, 'Thành phố Thủ Đức', 79);
INSERT INTO public.district (id, name, province_id) VALUES (770, 'Quận 3', 79);
INSERT INTO public.district (id, name, province_id) VALUES (771, 'Quận 10', 79);
INSERT INTO public.district (id, name, province_id) VALUES (772, 'Quận 11', 79);
INSERT INTO public.district (id, name, province_id) VALUES (773, 'Quận 4', 79);
INSERT INTO public.district (id, name, province_id) VALUES (774, 'Quận 5', 79);
INSERT INTO public.district (id, name, province_id) VALUES (775, 'Quận 6', 79);
INSERT INTO public.district (id, name, province_id) VALUES (776, 'Quận 8', 79);
INSERT INTO public.district (id, name, province_id) VALUES (777, 'Quận Bình Tân', 79);
INSERT INTO public.district (id, name, province_id) VALUES (778, 'Quận 7', 79);
INSERT INTO public.district (id, name, province_id) VALUES (783, 'Huyện Củ Chi', 79);
INSERT INTO public.district (id, name, province_id) VALUES (784, 'Huyện Hóc Môn', 79);
INSERT INTO public.district (id, name, province_id) VALUES (785, 'Huyện Bình Chánh', 79);
INSERT INTO public.district (id, name, province_id) VALUES (786, 'Huyện Nhà Bè', 79);
INSERT INTO public.district (id, name, province_id) VALUES (787, 'Huyện Cần Giờ', 79);

--
-- TOC entry 5082 (class 0 OID 21288)
-- Dependencies: 223
-- Data for Name: grade; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.grade (id, name, value, fee) VALUES (1, 'Lớp Lá', 1, '$100,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (3, 'Lớp 2', 2, '$200,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (2, 'Lớp 1', 3, '$200,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (4, 'Lớp 3', 4, '$200,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (5, 'Lớp 4', 5, '$200,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (6, 'Lớp 5', 6, '$200,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (7, 'Lớp 6', 7, '$300,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (8, 'Lớp 7', 8, '$300,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (9, 'Lớp 8', 9, '$300,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (10, 'Lớp 9', 10, '$300,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (11, 'Lớp 10', 11, '$400,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (12, 'Lớp 11', 12, '$400,000.00');
INSERT INTO public.grade (id, name, value, fee) VALUES (13, 'Lớp 12', 13, '$400,000.00');

--
-- TOC entry 5098 (class 0 OID 21326)
-- Dependencies: 239
-- Data for Name: status_type; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.status_type (id, type) VALUES (1, 'Register');
INSERT INTO public.status_type (id, type) VALUES (2, 'Form');
INSERT INTO public.status_type (id, type) VALUES (3, 'Queue');
INSERT INTO public.status_type (id, type) VALUES (4, 'Transaction');


--
-- TOC entry 5096 (class 0 OID 21322)
-- Dependencies: 237
-- Data for Name: status; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (1, 'pending', 1, 'Đang chờ');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (2, 'approval', 1, 'Đã được duyệt');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (3, 'deny', 1, 'Bị từ chối');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (4, 'update', 1, 'Cập nhật');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (5, 'pending', 2, 'Đang chờ');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (6, 'approval', 2, 'Đã được duyệt');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (8, 'handover', 2, 'Đã giao');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (9, 'cancel', 2, 'Đã hủy');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (10, 'pending', 3, 'Đang chờ');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (11, 'approval', 3, 'Đã được duyệt');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (12, 'deny', 3, 'Bị từ chối');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (13, 'handover', 3, 'Đã giao');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (15, 'refund', 3, 'Hoàn tiền');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (16, 'cancel', 3, 'Đã hủy');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (17, 'pending', 4, 'Đang chờ');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (18, 'paid', 4, 'Đã thanh toán');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (19, 'cancel', 4, 'Đã hủy');
INSERT INTO public.status (id, name, status_type_id, vietnamese_name) VALUES (7, 'deny', 2, 'Bị từ chối');

--
-- TOC entry 5100 (class 0 OID 21330)
-- Dependencies: 241
-- Data for Name: subject; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.subject (id, name, value) VALUES (2, 'Lý', 1);
INSERT INTO public.subject (id, name, value) VALUES (4, 'Văn', 2);
INSERT INTO public.subject (id, name, value) VALUES (1, 'Toán', 3);
INSERT INTO public.subject (id, name, value) VALUES (3, 'Hóa', 4);
INSERT INTO public.subject (id, name, value) VALUES (5, 'Tiếng Việt', 5);
INSERT INTO public.subject (id, name, value) VALUES (6, 'Anh Văn', 6);
INSERT INTO public.subject (id, name, value) VALUES (7, 'Báo bài', 7);
INSERT INTO public.subject (id, name, value) VALUES (8, 'Sinh', 8);
INSERT INTO public.subject (id, name, value) VALUES (9, 'Sử', 9);
INSERT INTO public.subject (id, name, value) VALUES (10, 'Địa', 10);

--
-- TOC entry 5094 (class 0 OID 21318)
-- Dependencies: 235
-- Data for Name: session_date; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.session_date (id, name, value) VALUES (1, 'Sáng Thứ 2', 1);
INSERT INTO public.session_date (id, name, value) VALUES (8, 'Chiều Thứ 2', 2);
INSERT INTO public.session_date (id, name, value) VALUES (15, 'Tối Thứ 2', 3);
INSERT INTO public.session_date (id, name, value) VALUES (2, 'Sáng Thứ 3', 4);
INSERT INTO public.session_date (id, name, value) VALUES (9, 'Chiều Thứ 3', 5);
INSERT INTO public.session_date (id, name, value) VALUES (16, 'Tối Thứ 3', 6);
INSERT INTO public.session_date (id, name, value) VALUES (3, 'Sáng Thứ 4', 7);
INSERT INTO public.session_date (id, name, value) VALUES (10, 'Chiều Thứ 4', 8);
INSERT INTO public.session_date (id, name, value) VALUES (17, 'Tối Thứ 4', 9);
INSERT INTO public.session_date (id, name, value) VALUES (4, 'Sáng Thứ 5', 10);
INSERT INTO public.session_date (id, name, value) VALUES (11, 'Chiều Thứ 5', 11);
INSERT INTO public.session_date (id, name, value) VALUES (18, 'Tối Thứ 5', 12);
INSERT INTO public.session_date (id, name, value) VALUES (5, 'Sáng Thứ 6', 13);
INSERT INTO public.session_date (id, name, value) VALUES (12, 'Chiều Thứ 6', 14);
INSERT INTO public.session_date (id, name, value) VALUES (19, 'Tối Thứ 6', 15);
INSERT INTO public.session_date (id, name, value) VALUES (6, 'Sáng Thứ 7', 16);
INSERT INTO public.session_date (id, name, value) VALUES (13, 'Chiều Thứ 7', 17);
INSERT INTO public.session_date (id, name, value) VALUES (20, 'Tối Thứ 7', 18);
INSERT INTO public.session_date (id, name, value) VALUES (7, 'Sáng CN', 19);
INSERT INTO public.session_date (id, name, value) VALUES (14, 'Chiều CN', 20);
INSERT INTO public.session_date (id, name, value) VALUES (21, 'Tối CN', 21);

--
-- TOC entry 5113 (class 0 OID 21374)
-- Dependencies: 254
-- Data for Name: tutor_type; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tutor_type (id, name, value) VALUES (1, 'Sinh viên', 1);
INSERT INTO public.tutor_type (id, name, value) VALUES (2, 'Giáo viên', 2);
INSERT INTO public.tutor_type (id, name, value) VALUES (3, 'Khác ', 3);


--
-- TOC entry 5123 (class 0 OID 0)
-- Dependencies: 220
-- Name: district_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.district_id_seq', 1, false);

--
-- TOC entry 5125 (class 0 OID 0)
-- Dependencies: 224
-- Name: grade_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.grade_id_seq', 20, true);



--
-- TOC entry 5127 (class 0 OID 0)
-- Dependencies: 228
-- Name: province_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.province_id_seq', 1, false);


--
-- TOC entry 5129 (class 0 OID 0)
-- Dependencies: 232
-- Name: requeststatus_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.requeststatus_id_seq', 1, true);


--
-- TOC entry 5130 (class 0 OID 0)
-- Dependencies: 234
-- Name: role_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.role_id_seq', 4, true);



--
-- TOC entry 5131 (class 0 OID 0)
-- Dependencies: 236
-- Name: sessiondate_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.sessiondate_id_seq', 26, true);


--
-- TOC entry 5132 (class 0 OID 0)
-- Dependencies: 238
-- Name: status_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.status_id_seq', 20, true);


--
-- TOC entry 5133 (class 0 OID 0)
-- Dependencies: 240
-- Name: statustype_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.statustype_id_seq', 4, true);


--
-- TOC entry 5134 (class 0 OID 0)
-- Dependencies: 242
-- Name: subject_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.subject_id_seq', 19, true);



--
-- TOC entry 5137 (class 0 OID 0)
-- Dependencies: 255
-- Name: tutor_type_tutor_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tutor_type_tutor_id_seq', 3, true);

