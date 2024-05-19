--
-- PostgreSQL database dump
--

-- Dumped from database version 16.2
-- Dumped by pg_dump version 16.2

-- Started on 2024-05-18 20:51:41

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
-- TOC entry 5 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: postgres
--

-- *not* creating schema, since initdb creates it


ALTER SCHEMA public OWNER TO postgres;

--
-- TOC entry 5099 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: postgres
--

COMMENT ON SCHEMA public IS '';


--
-- TOC entry 257 (class 1255 OID 18860)
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
-- TOC entry 269 (class 1255 OID 18861)
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
-- TOC entry 270 (class 1255 OID 18862)
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
-- TOC entry 215 (class 1259 OID 18863)
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
-- TOC entry 216 (class 1259 OID 18870)
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
-- TOC entry 5101 (class 0 OID 0)
-- Dependencies: 216
-- Name: account_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.account_id_seq OWNED BY public.account.id;


--
-- TOC entry 217 (class 1259 OID 18871)
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
-- TOC entry 218 (class 1259 OID 18875)
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
-- TOC entry 5102 (class 0 OID 0)
-- Dependencies: 218
-- Name: customer_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.customer_id_seq OWNED BY public.customer.id;


--
-- TOC entry 219 (class 1259 OID 18876)
-- Name: district; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.district (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    province_id integer NOT NULL
);


ALTER TABLE public.district OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 18879)
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
-- TOC entry 5103 (class 0 OID 0)
-- Dependencies: 220
-- Name: district_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.district_id_seq OWNED BY public.district.id;


--
-- TOC entry 221 (class 1259 OID 18880)
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
-- TOC entry 222 (class 1259 OID 18884)
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
-- TOC entry 5104 (class 0 OID 0)
-- Dependencies: 222
-- Name: employee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employee_id_seq OWNED BY public.employee.id;


--
-- TOC entry 223 (class 1259 OID 18885)
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
-- TOC entry 224 (class 1259 OID 18889)
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
-- TOC entry 5105 (class 0 OID 0)
-- Dependencies: 224
-- Name: grade_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.grade_id_seq OWNED BY public.grade.id;


--
-- TOC entry 225 (class 1259 OID 18890)
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
-- TOC entry 226 (class 1259 OID 18895)
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
-- TOC entry 5106 (class 0 OID 0)
-- Dependencies: 226
-- Name: identitycard_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.identitycard_id_seq OWNED BY public.identity_card.id;


--
-- TOC entry 227 (class 1259 OID 18896)
-- Name: province; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.province (
    id integer NOT NULL,
    name character varying(50) NOT NULL
);


ALTER TABLE public.province OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 18899)
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
-- TOC entry 5107 (class 0 OID 0)
-- Dependencies: 228
-- Name: province_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.province_id_seq OWNED BY public.province.id;


--
-- TOC entry 229 (class 1259 OID 18900)
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
-- TOC entry 230 (class 1259 OID 18906)
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
-- TOC entry 5108 (class 0 OID 0)
-- Dependencies: 230
-- Name: registerstatusdetail_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.registerstatusdetail_id_seq OWNED BY public.tutor_status_detail.id;


--
-- TOC entry 231 (class 1259 OID 18907)
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
-- TOC entry 232 (class 1259 OID 18913)
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
    CONSTRAINT tutorrequestform_students_check CHECK (((students > 0) AND (students < 5)))
);


ALTER TABLE public.request_tutor_form OWNER TO postgres;

--
-- TOC entry 233 (class 1259 OID 18920)
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
-- TOC entry 5109 (class 0 OID 0)
-- Dependencies: 233
-- Name: requeststatus_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.requeststatus_id_seq OWNED BY public.request_status.id;


--
-- TOC entry 234 (class 1259 OID 18921)
-- Name: role; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.role (
    id integer NOT NULL,
    name character varying(20) NOT NULL
);


ALTER TABLE public.role OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 18924)
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
-- TOC entry 5110 (class 0 OID 0)
-- Dependencies: 235
-- Name: role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.role_id_seq OWNED BY public.role.id;


--
-- TOC entry 236 (class 1259 OID 18925)
-- Name: session_date; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.session_date (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    value integer NOT NULL
);


ALTER TABLE public.session_date OWNER TO postgres;

--
-- TOC entry 237 (class 1259 OID 18928)
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
-- TOC entry 5111 (class 0 OID 0)
-- Dependencies: 237
-- Name: sessiondate_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.sessiondate_id_seq OWNED BY public.session_date.id;


--
-- TOC entry 238 (class 1259 OID 18929)
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
-- TOC entry 239 (class 1259 OID 18932)
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
-- TOC entry 5112 (class 0 OID 0)
-- Dependencies: 239
-- Name: status_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.status_id_seq OWNED BY public.status.id;


--
-- TOC entry 240 (class 1259 OID 18933)
-- Name: status_type; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.status_type (
    id integer NOT NULL,
    type character varying(50) NOT NULL
);


ALTER TABLE public.status_type OWNER TO postgres;

--
-- TOC entry 241 (class 1259 OID 18936)
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
-- TOC entry 5113 (class 0 OID 0)
-- Dependencies: 241
-- Name: statustype_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.statustype_id_seq OWNED BY public.status_type.id;


--
-- TOC entry 242 (class 1259 OID 18937)
-- Name: subject; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.subject (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    value integer NOT NULL
);


ALTER TABLE public.subject OWNER TO postgres;

--
-- TOC entry 243 (class 1259 OID 18940)
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
-- TOC entry 5114 (class 0 OID 0)
-- Dependencies: 243
-- Name: subject_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.subject_id_seq OWNED BY public.subject.id;


--
-- TOC entry 244 (class 1259 OID 18941)
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
-- TOC entry 245 (class 1259 OID 18945)
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
-- TOC entry 5115 (class 0 OID 0)
-- Dependencies: 245
-- Name: transactionhistory_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.transactionhistory_id_seq OWNED BY public.transaction_history.id;


--
-- TOC entry 246 (class 1259 OID 18946)
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
-- TOC entry 247 (class 1259 OID 18954)
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
-- TOC entry 248 (class 1259 OID 18958)
-- Name: tutor_grade; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_grade (
    tutor_id integer NOT NULL,
    grade_id integer NOT NULL
);


ALTER TABLE public.tutor_grade OWNER TO postgres;

--
-- TOC entry 249 (class 1259 OID 18961)
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
-- TOC entry 5116 (class 0 OID 0)
-- Dependencies: 249
-- Name: tutor_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tutor_id_seq OWNED BY public.tutor.id;


--
-- TOC entry 250 (class 1259 OID 18962)
-- Name: tutor_request_session; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_request_session (
    tutor_request_id integer NOT NULL,
    session_id integer NOT NULL
);


ALTER TABLE public.tutor_request_session OWNER TO postgres;

--
-- TOC entry 251 (class 1259 OID 18965)
-- Name: tutor_session; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_session (
    tutor_id integer NOT NULL,
    session_id integer NOT NULL
);


ALTER TABLE public.tutor_session OWNER TO postgres;

--
-- TOC entry 252 (class 1259 OID 18968)
-- Name: tutor_subject; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_subject (
    tutor_id integer NOT NULL,
    subject_id integer NOT NULL
);


ALTER TABLE public.tutor_subject OWNER TO postgres;

--
-- TOC entry 253 (class 1259 OID 18971)
-- Name: tutor_teaching_area; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_teaching_area (
    tutor_id integer NOT NULL,
    district_id integer NOT NULL
);


ALTER TABLE public.tutor_teaching_area OWNER TO postgres;

--
-- TOC entry 254 (class 1259 OID 18974)
-- Name: tutor_type; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tutor_type (
    id integer NOT NULL,
    name character varying(20) NOT NULL,
    value integer NOT NULL
);


ALTER TABLE public.tutor_type OWNER TO postgres;

--
-- TOC entry 255 (class 1259 OID 18977)
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
-- TOC entry 5117 (class 0 OID 0)
-- Dependencies: 255
-- Name: tutor_type_tutor_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tutor_type_tutor_id_seq OWNED BY public.tutor_type.id;


--
-- TOC entry 256 (class 1259 OID 18978)
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
-- TOC entry 5118 (class 0 OID 0)
-- Dependencies: 256
-- Name: tutorrequestform_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tutorrequestform_id_seq OWNED BY public.request_tutor_form.id;


--
-- TOC entry 4746 (class 2604 OID 18979)
-- Name: account id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account ALTER COLUMN id SET DEFAULT nextval('public.account_id_seq'::regclass);


--
-- TOC entry 4749 (class 2604 OID 18980)
-- Name: customer id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer ALTER COLUMN id SET DEFAULT nextval('public.customer_id_seq'::regclass);


--
-- TOC entry 4750 (class 2604 OID 18981)
-- Name: district id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.district ALTER COLUMN id SET DEFAULT nextval('public.district_id_seq'::regclass);


--
-- TOC entry 4751 (class 2604 OID 18982)
-- Name: employee id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee ALTER COLUMN id SET DEFAULT nextval('public.employee_id_seq'::regclass);


--
-- TOC entry 4752 (class 2604 OID 18983)
-- Name: grade id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grade ALTER COLUMN id SET DEFAULT nextval('public.grade_id_seq'::regclass);


--
-- TOC entry 4754 (class 2604 OID 18984)
-- Name: identity_card id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.identity_card ALTER COLUMN id SET DEFAULT nextval('public.identitycard_id_seq'::regclass);


--
-- TOC entry 4755 (class 2604 OID 18985)
-- Name: province id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.province ALTER COLUMN id SET DEFAULT nextval('public.province_id_seq'::regclass);


--
-- TOC entry 4758 (class 2604 OID 18986)
-- Name: request_status id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_status ALTER COLUMN id SET DEFAULT nextval('public.requeststatus_id_seq'::regclass);


--
-- TOC entry 4760 (class 2604 OID 18987)
-- Name: request_tutor_form id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form ALTER COLUMN id SET DEFAULT nextval('public.tutorrequestform_id_seq'::regclass);


--
-- TOC entry 4762 (class 2604 OID 18988)
-- Name: role id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role ALTER COLUMN id SET DEFAULT nextval('public.role_id_seq'::regclass);


--
-- TOC entry 4763 (class 2604 OID 18989)
-- Name: session_date id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.session_date ALTER COLUMN id SET DEFAULT nextval('public.sessiondate_id_seq'::regclass);


--
-- TOC entry 4764 (class 2604 OID 18990)
-- Name: status id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status ALTER COLUMN id SET DEFAULT nextval('public.status_id_seq'::regclass);


--
-- TOC entry 4765 (class 2604 OID 18991)
-- Name: status_type id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status_type ALTER COLUMN id SET DEFAULT nextval('public.statustype_id_seq'::regclass);


--
-- TOC entry 4766 (class 2604 OID 18992)
-- Name: subject id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subject ALTER COLUMN id SET DEFAULT nextval('public.subject_id_seq'::regclass);


--
-- TOC entry 4767 (class 2604 OID 18993)
-- Name: transaction_history id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history ALTER COLUMN id SET DEFAULT nextval('public.transactionhistory_id_seq'::regclass);


--
-- TOC entry 4769 (class 2604 OID 18994)
-- Name: tutor id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor ALTER COLUMN id SET DEFAULT nextval('public.tutor_id_seq'::regclass);


--
-- TOC entry 4756 (class 2604 OID 18995)
-- Name: tutor_status_detail id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_status_detail ALTER COLUMN id SET DEFAULT nextval('public.registerstatusdetail_id_seq'::regclass);


--
-- TOC entry 4772 (class 2604 OID 18996)
-- Name: tutor_type id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_type ALTER COLUMN id SET DEFAULT nextval('public.tutor_type_tutor_id_seq'::regclass);


--
-- TOC entry 5052 (class 0 OID 18863)
-- Dependencies: 215
-- Data for Name: account; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (1, 'superadmin@gmail.com', '0868273914', '$2a$11$oQPFccYFNCwP3wTlKuHcveqWRU6qTFSulE4N9/vDVh2Un5T9mf3Hq', false, 'https://media.tenor.com/RtmcggFXF04AAAAe/cat-kitten.png', '2024-05-11 22:07:43.524694', 1);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (2, 'nhanvien@gmail.com', '0999999999', '$2a$11$H5wqXrmnleTGwIPPoFQdzeL8T5.WZP2Qb.usX9/M6sRs0RsIkQLYu', false, 'https://icons.veryicon.com/png/o/miscellaneous/rookie-official-icon-gallery/225-default-avatar.png', '2024-05-11 22:09:46.319965', 2);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (3, 'nhanvien1@gmail.com', '0999999998', '$2a$11$wbR81fvolYs74lvp09SsN.fSXGDobSfVqbhhoq1E3GrpuhkOgF3Dm', false, 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2Fed706f6a-c609-44cc-821f-260db91cd04c.jpg?alt=media&token=76ddacac-e61b-4a81-9a06-9a93aae40286', '2024-05-11 22:11:10.700226', 2);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (4, 'nhanvien2@gmail.com', '0999999997', '$2a$11$LU.h7yQQZcDUieCwzkjVJOnkSDrBmF1F7L3OiYLjRhDSOlNmquK/y', false, 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2F3831648f-227d-4520-8e09-6206e2b32533.jpg?alt=media&token=fb150753-9dd8-4a66-bd6d-0d1bd120c6c1', '2024-05-11 22:12:48.043579', 2);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (5, 'nhanvien3@gmail.com', '0999999996', '$2a$11$QxgHMGuKE4y2lzFYpK4dGeAOPLChlPYwvTJPTPKWx/8RuCWt6f.5a', false, 'https://icons.veryicon.com/png/o/miscellaneous/rookie-official-icon-gallery/225-default-avatar.png', '2024-05-11 22:13:59.295726', 2);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (6, 'nhanvien4@gmail.com', '0999999995', '$2a$11$YUSFbzWmHSdki3vfwlu93.dN1xDIeFFarzJxZw4Fvv0tA60EwxoPK', false, 'https://icons.veryicon.com/png/o/miscellaneous/rookie-official-icon-gallery/225-default-avatar.png', '2024-05-11 22:15:16.209226', 2);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (7, 'giasu@gmail.com', '0888888888', '$2a$11$uqbos9pfofd4THFAzkUjHu8a52Y6b1mXXF0v5YnqJMeR2BDvcxWv.', false, 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2F1a72eae7-4dad-4397-b9fe-daabd4b47383.jpg?alt=media&token=6b09ffb4-9a15-4b72-914e-e7f9079cb803', '2024-05-11 22:21:27.270864', 3);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (8, 'giasu1@gmail.com', '0888888889', '$2a$11$/F7NRaoONqgeJyPYNSK05u1WY0zy29B2qIIYx.ohgHxtik2mDTyGW', false, 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2Fdf0120ce-032d-4cc2-a0b6-51dafe2c107b.jpg?alt=media&token=10d45cb6-f6d6-4547-ae48-7c9230124096', '2024-05-11 22:23:33.240956', 3);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (9, 'giasu2@gmail.com', '0888888887', '$2a$11$1a5LC2hQs8xWIUf6miuMBuOYDNOWmOBTgnVSypk3UWzE7C5I.0hrC', false, 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2F1ff602e5-4b6c-43a7-9af0-b6398f66e5e1.jpg?alt=media&token=c868f5bc-e021-4226-9c88-f41281757aa7', '2024-05-11 22:36:56.782797', 3);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (10, 'giasu4@gmail.com', '0888888886', '$2a$11$U/YvCYoL10tG0WFOXldgFOwIzzr3.08kEb1Zw1lxulcyuC.jetIha', false, 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2F04430841-7f3a-4d35-acf3-12bf738f7f82.jpg?alt=media&token=09f8b7b0-d780-432c-926f-b442234381f9', '2024-05-11 22:38:53.111104', 3);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (11, 'giasu3@gmail.com', '0888888885', '$2a$11$PmTEuH9NoK9WEyXs1ieLC.tKQUa5DyVp/hOnjLLQkrhd.E/lljmyC', false, 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2Fba1fb6b8-9419-4ed8-816b-d69165f28995.jpg?alt=media&token=105b8cda-f120-4d1d-8982-cfab96a3590d', '2024-05-11 22:41:15.951093', 3);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (12, 'khachhang@gmail.com', '0777777777', '$2a$11$ZCkREK9dlBIKdQeGXnVySekQN3Ye2cmA/Ggo9KVwwn/4uIFXfrRxi', false, 'https://icons.veryicon.com/png/o/miscellaneous/rookie-official-icon-gallery/225-default-avatar.png', '2024-05-11 22:59:29.848683', 4);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (13, 'giasu5@gmail.com', '0888888884', '$2a$11$OpCR28nj8DM6OONRsVPYWu7vdHayvEVCziA6xBl0OVq2/vCdIxAXG', false, 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2Fd824c9e5-ef6e-4879-a567-bf81388ea0d6.png?alt=media&token=c89f56e5-55e2-4604-b0a8-4cd6045eda6e', '2024-05-15 13:51:56.221601', 3);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (14, 'maihongthanh@GMAIL.COM', '0888888883', '$2a$11$Nel10Fsbplqh1TRWTWFa/OcMvoE1K4u6jUVSgUezC4bef2L0L.zNe', false, 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2Ff7140f7d-4086-4b51-b5f1-afe0bd8ddd48.png?alt=media&token=8f2e469d-e305-411a-adda-5faceaf7d99f', '2024-05-15 14:00:08.963882', 3);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (15, 'mylan@gmail.com', '0888888882', '$2a$11$/DtQXla55a2vbQN7kTI54O.e1SXbkixu5/FrwgqEMaKTpGUYY9.K6', false, 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2F0623e5a0-3ec2-42aa-9934-0662e448ba5f.png?alt=media&token=4420a39a-044e-4052-a7b6-857bef206d59', '2024-05-15 14:35:57.419078', 3);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (16, 'khachhang1@gmail.com', '0777777776', '$2a$11$u4KCQfCzOb/xAiptuxTy0.cFFL1h4oMNYe88MSIodkGCyW1Unuzaa', false, 'https://icons.veryicon.com/png/o/miscellaneous/rookie-official-icon-gallery/225-default-avatar.png', '2024-05-15 22:45:30.34349', 4);
INSERT INTO public.account (id, email, phone, password_hash, lock_enable, avatar, create_date, role_id) VALUES (17, 'khachhang3@gmail.com', '0777777775', '$2a$11$M41itK0zE92ondRjwVCla.t9AvXsJBP69eiBIDK9YBtlQlvMPNmqW', false, 'https://icons.veryicon.com/png/o/miscellaneous/rookie-official-icon-gallery/225-default-avatar.png', '2024-05-15 22:46:44.600676', 4);


--
-- TOC entry 5054 (class 0 OID 18871)
-- Dependencies: 217
-- Data for Name: customer; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.customer (id, full_name, birth, gender, address_detail, district_id, account_id, identity_id) VALUES (1, 'Tân Hoan', '2002-02-09', 'M', '123 Ninh Hòa', 282, 12, 12);
INSERT INTO public.customer (id, full_name, birth, gender, address_detail, district_id, account_id, identity_id) VALUES (2, 'Nguyễn Lan Man', '2001-09-09', 'F', 'Nguyễn bỉnh khiêm, Phường Đa kao', 760, 16, 16);
INSERT INTO public.customer (id, full_name, birth, gender, address_detail, district_id, account_id, identity_id) VALUES (3, 'Trần Thức', '2003-09-09', 'M', '123 khánh hòa', 5, 17, 17);


--
-- TOC entry 5056 (class 0 OID 18876)
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
-- TOC entry 5058 (class 0 OID 18880)
-- Dependencies: 221
-- Data for Name: employee; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.employee (id, full_name, birth, gender, address_detail, district_id, account_id, identity_id) VALUES (1, 'SuperAdmin Nguyen', '2002-05-31', 'M', 'Nowhere', 568, 1, 1);
INSERT INTO public.employee (id, full_name, birth, gender, address_detail, district_id, account_id, identity_id) VALUES (2, 'Nguyễn Hồng Anh', '2002-02-01', 'M', 'Tây Hồ', 3, 2, 2);
INSERT INTO public.employee (id, full_name, birth, gender, address_detail, district_id, account_id, identity_id) VALUES (3, 'Nguyễn Hồng Mỹ', '2003-02-01', 'F', 'Đông Anh', 760, 3, 3);
INSERT INTO public.employee (id, full_name, birth, gender, address_detail, district_id, account_id, identity_id) VALUES (4, 'Nguyễn Thị Lan', '1999-02-01', 'F', 'Ninh Hòa', 17, 4, 4);
INSERT INTO public.employee (id, full_name, birth, gender, address_detail, district_id, account_id, identity_id) VALUES (5, 'Trần Mai Anh', '2002-02-09', 'F', 'Hà Nội', 569, 5, 5);
INSERT INTO public.employee (id, full_name, birth, gender, address_detail, district_id, account_id, identity_id) VALUES (6, 'Cao Văn Lan', '2001-02-09', 'M', '123 khánh hòa', 570, 6, 6);


--
-- TOC entry 5060 (class 0 OID 18885)
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
-- TOC entry 5062 (class 0 OID 18890)
-- Dependencies: 225
-- Data for Name: identity_card; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (1, '123456789', 'https://media.tenor.com/RtmcggFXF04AAAAe/cat-kitten.png', 'https://media.tenor.com/RtmcggFXF04AAAAe/cat-kitten.png');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (2, '999999999', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F99aed2d5-72e8-4209-9e07-2d63f0a9c7e7.jpg?alt=media&token=5b26ebdf-98a4-4ec1-a668-d5221ca4798e', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F49a6be4e-8e2a-4550-bca8-44b89f677ae2.jpg?alt=media&token=ddf98efa-4e4c-43a0-bf63-8453404015c1');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (3, '999999998', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F91de2f6b-54ee-48b3-8a27-837fe1ad5a1e.jpg?alt=media&token=4dc97266-9a39-4472-a0ba-838b989916c2', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fa45592e2-2edd-48ac-a156-5a0ff340e638.jpg?alt=media&token=944525ed-9cef-4042-a0ed-4d3145d19f21');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (4, '999999997', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F64cc78db-bb52-4a22-a47e-ae3b95ee4932.jpg?alt=media&token=e52ff402-282e-4249-a99d-717b799388c5', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F6390a8ed-b5ea-4304-85c8-bb9767b5d855.png?alt=media&token=de1f13a0-cc69-430d-b01f-3785a7e654dc');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (5, '999999996', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F2a9a56f2-8bd5-4eb8-9a1f-9c68740f7d0a.jpg?alt=media&token=06b9b351-72d2-4c10-805c-2feb3c739af9', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F1d406fd1-bd4d-44ae-a077-3d4f5b18cd20.jpg?alt=media&token=189c816b-4a72-4cd1-b7e9-11d316cd5802');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (6, '999999995', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F505a9784-9306-4fe6-8882-e38f0ecd8e62.jpg?alt=media&token=7a2c8293-8276-4ee1-8d43-7886db7f0ab8', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F9af65425-835f-4755-b35c-44587f2f0c99.jpg?alt=media&token=88e36551-accc-42ab-8ca3-b025f32eab4e');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (7, '888888888', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fc6e1e585-b849-4646-b4ff-c4ad05b3fd33.jpg?alt=media&token=e640d0f2-a1f1-489d-a3e2-6895b961be2d', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F6dac9811-7c11-4b5b-aa3f-c8963879cb7e.jpg?alt=media&token=728a7f47-7562-4c33-a856-fc850f479445');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (8, '888888889', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F838e07df-4103-47d2-ac1e-ea168d283840.jpg?alt=media&token=bdff70f0-a78d-4b42-bd31-7ffa0fe6b204', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F9dfb0331-56dd-4b84-8a48-17729f3d7a43.jpg?alt=media&token=0a29992a-c3c5-4961-89fc-6580bef9fd69');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (9, '888888887', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fbf8b0522-a536-415d-83b8-2663db3351b3.jpg?alt=media&token=6e5eca23-bb76-4e58-841a-c762e9fb25dd', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F515e871f-b9e6-45b2-bbe4-422bc6548e8c.jpg?alt=media&token=c0b9820a-f1cf-4703-ab22-eb74e3981380');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (10, '888888886', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fb78af760-7e0c-4a12-b48d-d8dac0b9db96.jpg?alt=media&token=340925d6-529b-4832-b514-1d962e058923', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fe1397537-7f26-4a1b-b1c6-979bcadaf431.jpg?alt=media&token=f3dcc17f-468d-46ea-a609-c75c20641ee8');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (11, '888888885', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F2ae69a72-529d-46f6-b115-80aa00084a0d.jpg?alt=media&token=dafbfb73-df16-4fe1-8e62-fb1b05ea29da', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fecfe22cf-4211-42ea-a378-89af3934b8f9.jpg?alt=media&token=68439996-ea11-4ced-b0b0-ec282b1a4c24');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (12, '777777777', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fbf53f4f4-921f-4692-9b2f-47151604b5ac.jpg?alt=media&token=76fa1906-02aa-4656-991f-1d3a9bcafde8', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fdda619b6-823f-4986-b8be-b0b92d49e96f.jpg?alt=media&token=5d3c8a63-be0e-494f-90e0-63fc6abf6a91');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (13, '888888884', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F37a51421-0b6a-494f-9a4d-fdd915511878.jpg?alt=media&token=8540afe0-d9ed-45e5-a233-6b1db0f2fdea', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F1ab42a93-01d8-461a-a904-444abd595c22.jpg?alt=media&token=a6bb918b-0e49-40ab-8b8e-35c4cb9a34f8');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (14, '888888883', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F8b73c243-360e-4551-80ae-26876cf4300d.jpg?alt=media&token=4855c3e3-8e81-4b6b-af90-6cb4faad343d', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F151ae2e7-e643-4367-854d-bffe7d12562e.jpg?alt=media&token=c985421d-a847-4ade-a064-324a057fae15');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (15, '888888882', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fa48ab1de-6113-492b-b195-ab5318be1612.jpg?alt=media&token=8ba38916-96dd-40a0-bdfb-1a12ece344ae', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F5c5cb9ac-67d1-4393-933c-6700a76c8ad7.jpg?alt=media&token=b87005ce-68ec-499b-8651-16c58c444644');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (16, '777777776', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F9a4e08f2-4103-4038-9d59-cf6637e24efa.jpg?alt=media&token=6293fad2-ba69-4b7e-a023-d487ac36a15d', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F225c7c15-25f5-4b43-96c3-747197efac58.jpg?alt=media&token=04cbbf43-394d-4b4e-a7ff-2aae0349f6de');
INSERT INTO public.identity_card (id, identity_number, front_identity_card, back_identity_card) VALUES (17, '777777775', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fd626fe9d-797a-4064-9518-814feb4dd6de.jpg?alt=media&token=255fd80e-7c34-4e56-95b1-5aa797a91bc9', 'https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F33b4795b-daa0-4ba0-a2b8-8c0984893c27.jpg?alt=media&token=d3dc8788-f264-4ca1-b905-6cb7c16cb1cc');


--
-- TOC entry 5064 (class 0 OID 18896)
-- Dependencies: 227
-- Data for Name: province; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.province (id, name) VALUES (1, 'Thành phố Hà Nội');
INSERT INTO public.province (id, name) VALUES (56, 'Tỉnh Khánh Hòa');
INSERT INTO public.province (id, name) VALUES (79, 'Thành phố Hồ Chí Minh');


--
-- TOC entry 5068 (class 0 OID 18907)
-- Dependencies: 231
-- Data for Name: request_status; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- TOC entry 5069 (class 0 OID 18913)
-- Dependencies: 232
-- Data for Name: request_tutor_form; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (2, 2, NULL, '2024-05-15 14:17:39.162885', '2024-06-14 14:17:39.162887', '97 Man Thiện, HIệp Phú', 1, 13, 6, 275, 1);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (3, 2, NULL, '2024-05-15 14:25:12.219143', '2024-06-14 14:25:12.219144', '123 Hàng Mai', 2, 1, 8, 3, 1);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (13, 2, NULL, '2024-05-18 15:40:16.761598', '2024-06-17 15:40:16.761602', '123 khánh hòa', 8, 13, 6, 570, 2);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (12, 3, NULL, '2024-05-18 15:39:46.517819', '2024-06-17 15:39:46.517822', '123 khánh sơn', 7, 1, 6, 760, 2);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (11, 4, NULL, '2024-05-18 15:39:17.409132', '2024-06-17 15:39:17.409134', '123 lê lai', 6, 7, 6, 786, 2);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (9, 2, NULL, '2024-05-18 15:38:16.429606', '2024-06-17 15:38:16.429611', 'Nguyễn bỉnh khiêm, Phường Đa kao', 2, 1, 6, 766, 2);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (8, 2, NULL, '2024-05-18 15:38:02.539889', '2024-06-17 15:38:02.53989', 'Nguyễn bỉnh khiêm, Phường Đa kao', 2, 8, 6, 760, 2);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (7, 1, NULL, '2024-05-18 15:37:43.605579', '2024-06-17 15:37:43.605802', 'Nguyễn bỉnh khiêm, Phường Đa kao', 2, 9, 6, 760, 2);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (14, 1, NULL, '2024-05-18 15:41:52.402989', '2024-06-17 15:41:52.402991', '123 khánh hòa', 9, 11, 5, 5, 3);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (15, 1, NULL, '2024-05-18 15:42:07.792541', '2024-06-17 15:42:07.792542', '123 khánh hòa', 6, 11, 5, 5, 3);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (16, 1, NULL, '2024-05-18 15:42:23.774825', '2024-06-17 15:42:23.774828', '123 khánh hòa', 7, 1, 5, 760, 3);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (17, 1, NULL, '2024-05-18 15:43:48.830458', '2024-06-17 15:43:48.83046', '123 khánh lâm', 10, 11, 5, 5, 3);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (18, 3, NULL, '2024-05-18 15:44:15.386826', '2024-06-17 15:44:15.386828', '123 mỹ sơn', 3, 7, 6, 768, 3);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (4, 1, NULL, '2024-05-16 15:14:26.889285', '2024-06-15 15:14:26.889516', '123 Nguyễn tri phương', 5, 8, 6, 18, 3);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (5, 2, NULL, '2024-05-16 15:14:54.559281', '2024-06-15 15:14:54.559285', '97 Man Thiện, HIệp Phú', 6, 6, 6, 3, 3);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (6, 3, NULL, '2024-05-16 15:15:19.852474', '2024-06-15 15:15:19.852586', '123 khánh sơn', 8, 8, 6, 575, 3);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (1, 1, NULL, '2024-05-15 14:14:47.361092', '2024-06-14 14:14:47.362476', 'Đoàn Thị Điểm', 2, 11, 8, 20, 1);
INSERT INTO public.request_tutor_form (id, students, additional_detail, create_date, expired_date, address_detail, subject_id, grade_id, status_id, district_id, customer_id) VALUES (19, 1, NULL, '2024-05-18 20:30:40.694245', '2024-06-17 20:30:40.694359', '123 nInH hÒa', 2, 4, 5, 282, 1);


--
-- TOC entry 5071 (class 0 OID 18921)
-- Dependencies: 234
-- Data for Name: role; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.role (id, name) VALUES (1, 'admin');
INSERT INTO public.role (id, name) VALUES (2, 'employee');
INSERT INTO public.role (id, name) VALUES (3, 'tutor');
INSERT INTO public.role (id, name) VALUES (4, 'customer');


--
-- TOC entry 5073 (class 0 OID 18925)
-- Dependencies: 236
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
-- TOC entry 5075 (class 0 OID 18929)
-- Dependencies: 238
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
-- TOC entry 5077 (class 0 OID 18933)
-- Dependencies: 240
-- Data for Name: status_type; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.status_type (id, type) VALUES (1, 'Register');
INSERT INTO public.status_type (id, type) VALUES (2, 'Form');
INSERT INTO public.status_type (id, type) VALUES (3, 'Queue');
INSERT INTO public.status_type (id, type) VALUES (4, 'Transaction');


--
-- TOC entry 5079 (class 0 OID 18937)
-- Dependencies: 242
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
-- TOC entry 5081 (class 0 OID 18941)
-- Dependencies: 244
-- Data for Name: transaction_history; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.transaction_history (id, create_date, payment_date, payment_amount, context, employee_id, tutor_id, form_id, type_transaction, status_id) VALUES (1, '2024-05-16 15:50:19.690913', '2024-05-16 15:51:00', '$400,000.00', 'Đóng phí nhận lớp, ngay sau khí đóng hãy gửi hình ảnh qua zalo của nhân viên Nguyễn Hồng Anh: 0999999999', 2, 2, 1, true, 18);
INSERT INTO public.transaction_history (id, create_date, payment_date, payment_amount, context, employee_id, tutor_id, form_id, type_transaction, status_id) VALUES (2, '2024-05-16 21:38:18.73932', NULL, '$400,000.00', 'Đóng phí nhận lớp, ngay sau khí đóng hãy gửi hình ảnh qua zalo của nhân viên Nguyễn Hồng Mỹ: 0999999998', 3, 2, 2, true, 17);
INSERT INTO public.transaction_history (id, create_date, payment_date, payment_amount, context, employee_id, tutor_id, form_id, type_transaction, status_id) VALUES (4, '2024-05-18 14:38:53.333313', NULL, '$100,000.00', 'Đóng phí nhận lớp, ngay sau khí đóng hãy gửi hình ảnh qua zalo của nhân viên Nguyễn Hồng Mỹ: 0999999998', 3, 2, 3, true, 17);
INSERT INTO public.transaction_history (id, create_date, payment_date, payment_amount, context, employee_id, tutor_id, form_id, type_transaction, status_id) VALUES (3, '2024-05-18 14:38:46.851803', '2024-05-18 14:39:00', '$100,000.00', 'Đóng phí nhận lớp, ngay sau khí đóng hãy gửi hình ảnh qua zalo của nhân viên Nguyễn Hồng Mỹ: 0999999998', 3, 5, 3, true, 18);
INSERT INTO public.transaction_history (id, create_date, payment_date, payment_amount, context, employee_id, tutor_id, form_id, type_transaction, status_id) VALUES (6, '2024-05-18 20:22:24.603696', NULL, '$400,000.00', 'Đóng phí nhận lớp, ngay sau khí đóng hãy gửi hình ảnh qua zalo của nhân viên Nguyễn Hồng Mỹ: 0999999998', 3, 3, 1, true, 17);
INSERT INTO public.transaction_history (id, create_date, payment_date, payment_amount, context, employee_id, tutor_id, form_id, type_transaction, status_id) VALUES (5, '2024-05-18 20:22:02.873977', '2024-05-18 20:23:00', '$400,000.00', 'Đóng phí nhận lớp, ngay sau khí đóng hãy gửi hình ảnh qua zalo của nhân viên Nguyễn Hồng Mỹ: 0999999998', 3, 5, 1, true, 18);


--
-- TOC entry 5083 (class 0 OID 18946)
-- Dependencies: 246
-- Data for Name: tutor; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tutor (id, full_name, birth, gender, address_detail, college, area, additional_info, academic_year_from, academic_year_to, is_active, district_id, account_id, identity_id, tutor_type_id, status_id) VALUES (2, 'Trần Anh', '1998-01-01', 'M', '123 lê văn việt', 'Khxh Và Nv', '123', 'Có kinh nghiệm đi dạy nhiều năm', 1998, 2002, true, 8, 8, 8, 1, 2);
INSERT INTO public.tutor (id, full_name, birth, gender, address_detail, college, area, additional_info, academic_year_from, academic_year_to, is_active, district_id, account_id, identity_id, tutor_type_id, status_id) VALUES (1, 'Nguyễn Hồng Mai', '1999-01-01', 'F', '123 Nguyễn Trãi', 'Ptit', 'Cntt', 'Có thể dạy kèm thi học sinh giỏi', 2019, 2023, true, 572, 7, 7, 1, 4);
INSERT INTO public.tutor (id, full_name, birth, gender, address_detail, college, area, additional_info, academic_year_from, academic_year_to, is_active, district_id, account_id, identity_id, tutor_type_id, status_id) VALUES (7, 'Nguyễn Mai Hồng Thanh', '-infinity', 'M', '12 Lê Duẫn', 'Khtn', 'Toán Lý', 'Dạy ok', 2111, 2114, true, 3, 14, 14, 1, 1);
INSERT INTO public.tutor (id, full_name, birth, gender, address_detail, college, area, additional_info, academic_year_from, academic_year_to, is_active, district_id, account_id, identity_id, tutor_type_id, status_id) VALUES (4, 'Nguyễn Vy', '2019-01-01', 'M', '123 Nguyễn Trãi', 'Khxh Và Nv', 'Cơ Khí', NULL, 2019, 2022, true, 3, 10, 10, 3, 4);
INSERT INTO public.tutor (id, full_name, birth, gender, address_detail, college, area, additional_info, academic_year_from, academic_year_to, is_active, district_id, account_id, identity_id, tutor_type_id, status_id) VALUES (8, 'Nguyễn Mỹ Lan', '2001-01-01', 'F', '123 Nguyễn Trãi', 'Khxh Và Nv', 'Công Nghệ Thông Tin', 'Dạy tốt', 2019, 2029, true, 274, 15, 15, 1, 1);
INSERT INTO public.tutor (id, full_name, birth, gender, address_detail, college, area, additional_info, academic_year_from, academic_year_to, is_active, district_id, account_id, identity_id, tutor_type_id, status_id) VALUES (3, 'Ethan Lee', '1999-01-01', 'M', '123 Nguyễn Trãi', 'Khxh Và Nv', 'Khoa Học Máy Tính', 'Có thể dạy kèm thi hsg', 2018, 20119, true, 3, 9, 9, 1, 2);
INSERT INTO public.tutor (id, full_name, birth, gender, address_detail, college, area, additional_info, academic_year_from, academic_year_to, is_active, district_id, account_id, identity_id, tutor_type_id, status_id) VALUES (6, 'Nguyen Thi My Hanh', '2019-01-01', 'F', '123 Long Thanh', 'Nguyễn Tri Phương', 'Khoa Học Máy Tính', 'Có thể dạy kèm thi hsg', 2019, 2022, true, 3, 13, 13, 2, 2);
INSERT INTO public.tutor (id, full_name, birth, gender, address_detail, college, area, additional_info, academic_year_from, academic_year_to, is_active, district_id, account_id, identity_id, tutor_type_id, status_id) VALUES (5, 'Trần Thái Mỹ', '2002-01-01', 'M', '123 Nguyễn Hồng', 'Đại Học Sư Phạm Kĩ Thuật', 'Cơ Khí', 'Dạy ok', 2019, 2023, true, 773, 11, 11, 1, 4);


--
-- TOC entry 5084 (class 0 OID 18954)
-- Dependencies: 247
-- Data for Name: tutor_apply_form; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (2, 2, '2024-05-15 14:17:39.399094', 16);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (1, 2, '2024-05-16 15:48:54.836652', 16);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (3, 2, '2024-05-17 22:59:11.004543', 11);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (3, 5, '2024-05-15 14:25:12.453595', 8);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (4, 2, '2024-05-18 15:48:49.760432', 10);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (6, 2, '2024-05-18 15:48:52.178688', 10);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (11, 2, '2024-05-18 15:48:54.782807', 10);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (6, 3, '2024-05-18 20:20:14.23334', 10);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (5, 3, '2024-05-18 20:20:17.459435', 10);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (13, 3, '2024-05-18 20:20:20.328672', 10);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (11, 3, '2024-05-18 20:20:23.413317', 10);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (4, 5, '2024-05-18 20:21:04.087467', 10);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (8, 5, '2024-05-18 20:21:06.069752', 10);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (18, 5, '2024-05-18 20:21:08.493069', 10);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (1, 3, '2024-05-18 20:20:16.090737', 11);
INSERT INTO public.tutor_apply_form (tutor_request_id, tutor_id, enter_date, status_id) VALUES (1, 5, '2024-05-18 20:21:00.648551', 8);


--
-- TOC entry 5085 (class 0 OID 18958)
-- Dependencies: 248
-- Data for Name: tutor_grade; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 1);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 2);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 3);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 4);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 5);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 6);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 7);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 8);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 9);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 10);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 11);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 12);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (1, 13);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (2, 13);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (3, 4);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (3, 6);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (3, 8);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (4, 4);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (5, 6);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (6, 7);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (7, 7);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (3, 7);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (8, 10);
INSERT INTO public.tutor_grade (tutor_id, grade_id) VALUES (5, 13);


--
-- TOC entry 5087 (class 0 OID 18962)
-- Dependencies: 250
-- Data for Name: tutor_request_session; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (1, 4);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (2, 2);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (3, 1);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (4, 5);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (4, 11);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (4, 18);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (5, 4);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (5, 10);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (5, 17);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (6, 3);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (6, 10);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (6, 20);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (7, 4);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (7, 11);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (7, 18);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (8, 4);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (8, 9);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (9, 2);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (9, 13);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (11, 4);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (11, 12);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (12, 1);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (13, 4);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (13, 11);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (14, 5);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (14, 9);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (14, 17);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (15, 3);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (16, 4);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (16, 14);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (17, 4);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (18, 3);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (18, 14);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (19, 4);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (19, 12);
INSERT INTO public.tutor_request_session (tutor_request_id, session_id) VALUES (19, 16);


--
-- TOC entry 5088 (class 0 OID 18965)
-- Dependencies: 251
-- Data for Name: tutor_session; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 1);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 2);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 3);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 4);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 5);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 6);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 7);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 8);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 9);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 10);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 11);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 12);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 13);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 14);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 15);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 16);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 17);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 18);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 19);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 20);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (1, 21);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (2, 4);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (3, 6);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (3, 11);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (3, 17);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (4, 4);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (5, 5);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (6, 2);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (7, 6);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (8, 4);
INSERT INTO public.tutor_session (tutor_id, session_id) VALUES (5, 4);


--
-- TOC entry 5066 (class 0 OID 18900)
-- Dependencies: 229
-- Data for Name: tutor_status_detail; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (1, 'Tạo tài khoản', '2024-05-11 22:21:27.335267', 1, 1);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (2, 'Tạo tài khoản', '2024-05-11 22:23:33.278856', 2, 1);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (3, 'Tạo tài khoản', '2024-05-11 22:36:56.850035', 3, 1);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (4, 'Tạo tài khoản', '2024-05-11 22:38:53.145738', 4, 1);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (5, 'Tạo tài khoản', '2024-05-11 22:41:15.992651', 5, 1);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (6, 'duyệt tài khoản', '2024-05-11 22:52:07.477667', 1, 2);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (7, '{"TutorId":1,"Fullname":"Hồng Mai","Birth":"1999-01-01","Email":"giasu@gmail.com","Phone":"0888888888","Gender":"F","Avatar":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2F1a72eae7-4dad-4397-b9fe-daabd4b47383.jpg?alt=media&token=6b09ffb4-9a15-4b72-914e-e7f9079cb803","AddressDetail":"123 Nguyễn Trãi","SelectedProvinceId":56,"SelectedDistrictId":572,"IdentityCard":"888888888","FrontIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fc6e1e585-b849-4646-b4ff-c4ad05b3fd33.jpg?alt=media&token=e640d0f2-a1f1-489d-a3e2-6895b961be2d","BackIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F6dac9811-7c11-4b5b-aa3f-c8963879cb7e.jpg?alt=media&token=728a7f47-7562-4c33-a856-fc850f479445","College":"Ptit","Area":"Cntt","IsActive":true,"Additionalinfo":"Có thể dạy kèm thi hsg","Academicyearfrom":2019,"Academicyearto":2022,"SelectedTutorTypeId":1,"FormatAddress":"","FormatTutorType":"Sinh viên","FormatTeachingArea":"","FormatGrades":"","FormatSessions":"","FormatSubjects":"","SelectedSubjectIds":[1,2,3,4,5,6,7,8,9,10],"SelectedSessionIds":[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21],"SelectedDistricts":[568,569,570,571,572,573,574,575,576],"SelectedGradeIds":[1,2,3,4,5,6,7,8,9,10,11,12,13]}', '2024-05-11 22:53:20.904859', 1, 4);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (8, '{"TutorId":1,"Fullname":"Hồng Mai","Birth":"1999-01-01","Email":"giasu@gmail.com","Phone":"0888888888","Gender":"F","Avatar":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2F1a72eae7-4dad-4397-b9fe-daabd4b47383.jpg?alt=media&token=6b09ffb4-9a15-4b72-914e-e7f9079cb803","AddressDetail":"123 Nguyễn Trãi","SelectedProvinceId":56,"SelectedDistrictId":572,"IdentityCard":"888888888","FrontIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fc6e1e585-b849-4646-b4ff-c4ad05b3fd33.jpg?alt=media&token=e640d0f2-a1f1-489d-a3e2-6895b961be2d","BackIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F6dac9811-7c11-4b5b-aa3f-c8963879cb7e.jpg?alt=media&token=728a7f47-7562-4c33-a856-fc850f479445","College":"Ptit","Area":"Cntt","IsActive":true,"Additionalinfo":"Có thể dạy kèm thi hsg","Academicyearfrom":2019,"Academicyearto":2023,"SelectedTutorTypeId":1,"FormatAddress":"","FormatTutorType":"Sinh viên","FormatTeachingArea":"","FormatGrades":"","FormatSessions":"","FormatSubjects":"","SelectedSubjectIds":[1,2,3,4,5,6,7,8,9,10],"SelectedSessionIds":[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21],"SelectedDistricts":[568,569,570,571,572,573,574,575,576],"SelectedGradeIds":[1,2,3,4,5,6,7,8,9,10,11,12,13]}', '2024-05-11 22:56:09.719258', 1, 4);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (9, 'Duyet tai khoan', '2024-05-15 13:41:38.64874', 2, 2);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (10, 'duyet tai khoan', '2024-05-15 13:41:53.832782', 3, 2);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (11, 'duyet tai khoan', '2024-05-15 13:42:09.713448', 4, 2);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (12, 'duyet tai khoan', '2024-05-15 13:42:22.786607', 5, 2);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (13, 'duyet tai khoan', '2024-05-15 13:42:23.051448', 5, 2);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (14, 'Tạo tài khoản', '2024-05-15 13:51:56.308817', 6, 1);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (15, '{"TutorId":1,"Fullname":"Hồng Mai","Birth":"1999-01-01","Email":"giasu@gmail.com","Phone":"0888888888","Gender":"F","Avatar":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2F1a72eae7-4dad-4397-b9fe-daabd4b47383.jpg?alt=media&token=6b09ffb4-9a15-4b72-914e-e7f9079cb803","AddressDetail":"123 Nguyễn Trãi","SelectedProvinceId":56,"SelectedDistrictId":572,"IdentityCard":"888888888","FrontIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fc6e1e585-b849-4646-b4ff-c4ad05b3fd33.jpg?alt=media&token=e640d0f2-a1f1-489d-a3e2-6895b961be2d","BackIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F6dac9811-7c11-4b5b-aa3f-c8963879cb7e.jpg?alt=media&token=728a7f47-7562-4c33-a856-fc850f479445","College":"Ptit","Area":"Cntt","IsActive":true,"Additionalinfo":"Có thể dạy kèm thi học sinh giỏi","Academicyearfrom":2019,"Academicyearto":2023,"SelectedTutorTypeId":1,"FormatAddress":"","FormatTutorType":"Sinh viên","FormatTeachingArea":"","FormatGrades":"","FormatSessions":"","FormatSubjects":"","SelectedSubjectIds":[1,2,3,4,5,6,7,8,9,10],"SelectedSessionIds":[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21],"SelectedDistricts":[568,569,570,571,572,573,574,575,576],"SelectedGradeIds":[1,2,3,4,5,6,7,8,9,10,11,12,13]}', '2024-05-15 13:52:48.180448', 1, 4);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (16, 'Tạo tài khoản', '2024-05-15 14:00:09.034445', 7, 1);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (17, '{"TutorId":4,"Fullname":"Nguyễn Vy","Birth":"2019-01-01","Email":"giasu4@gmail.com","Phone":"0888888886","Gender":"M","Avatar":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2F04430841-7f3a-4d35-acf3-12bf738f7f82.jpg?alt=media&token=09f8b7b0-d780-432c-926f-b442234381f9","AddressDetail":"123 Nguyễn Trãi","SelectedProvinceId":1,"SelectedDistrictId":3,"IdentityCard":"888888886","FrontIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fb78af760-7e0c-4a12-b48d-d8dac0b9db96.jpg?alt=media&token=340925d6-529b-4832-b514-1d962e058923","BackIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fe1397537-7f26-4a1b-b1c6-979bcadaf431.jpg?alt=media&token=f3dcc17f-468d-46ea-a609-c75c20641ee8","College":"Khxh Và Nv","Area":"Cơ Khí","IsActive":true,"Additionalinfo":null,"Academicyearfrom":2019,"Academicyearto":2022,"SelectedTutorTypeId":3,"FormatAddress":"","FormatTutorType":"Khác ","FormatTeachingArea":"","FormatGrades":"","FormatSessions":"","FormatSubjects":"","SelectedSubjectIds":[10],"SelectedSessionIds":[4],"SelectedDistricts":[5,274],"SelectedGradeIds":[4]}', '2024-05-15 14:05:39.036241', 4, 4);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (18, '{"TutorId":3,"Fullname":"Ethan Lee","Birth":"1999-01-01","Email":"giasu2@gmail.com","Phone":"0888888887","Gender":"M","Avatar":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2F1ff602e5-4b6c-43a7-9af0-b6398f66e5e1.jpg?alt=media&token=c868f5bc-e021-4226-9c88-f41281757aa7","AddressDetail":"123 Nguyễn Trãi","SelectedProvinceId":1,"SelectedDistrictId":3,"IdentityCard":"888888887","FrontIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fbf8b0522-a536-415d-83b8-2663db3351b3.jpg?alt=media&token=6e5eca23-bb76-4e58-841a-c762e9fb25dd","BackIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F515e871f-b9e6-45b2-bbe4-422bc6548e8c.jpg?alt=media&token=c0b9820a-f1cf-4703-ab22-eb74e3981380","College":"Khxh Và Nv","Area":"Khoa Học Máy Tính","IsActive":true,"Additionalinfo":"Có thể dạy kèm thi hsg","Academicyearfrom":2018,"Academicyearto":20119,"SelectedTutorTypeId":1,"FormatAddress":"","FormatTutorType":"Sinh viên","FormatTeachingArea":"","FormatGrades":"","FormatSessions":"","FormatSubjects":"","SelectedSubjectIds":[3,5,6],"SelectedSessionIds":[6,11,17],"SelectedDistricts":[5,274,281],"SelectedGradeIds":[4,6,8]}', '2024-05-15 14:06:08.12909', 3, 4);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (21, '{"TutorId":3,"Fullname":"Ethan Lee","Birth":"1999-01-01","Email":"giasu2@gmail.com","Phone":"0888888887","Gender":"M","Avatar":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2F1ff602e5-4b6c-43a7-9af0-b6398f66e5e1.jpg?alt=media&token=c868f5bc-e021-4226-9c88-f41281757aa7","AddressDetail":"123 Nguyễn Trãi","SelectedProvinceId":1,"SelectedDistrictId":3,"IdentityCard":"888888887","FrontIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fbf8b0522-a536-415d-83b8-2663db3351b3.jpg?alt=media&token=6e5eca23-bb76-4e58-841a-c762e9fb25dd","BackIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F515e871f-b9e6-45b2-bbe4-422bc6548e8c.jpg?alt=media&token=c0b9820a-f1cf-4703-ab22-eb74e3981380","College":"Khxh Và Nv","Area":"Khoa Học Máy Tính","IsActive":true,"Additionalinfo":"Có thể dạy kèm thi hsg","Academicyearfrom":2018,"Academicyearto":20119,"SelectedTutorTypeId":1,"FormatAddress":"","FormatTutorType":"Sinh viên","FormatTeachingArea":"","FormatGrades":"","FormatSessions":"","FormatSubjects":"","SelectedSubjectIds":[3,5,6,10],"SelectedSessionIds":[6,11,17],"SelectedDistricts":[5,8,274,281],"SelectedGradeIds":[4,6,7,8]}', '2024-05-15 14:10:48.531653', 3, 4);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (22, 'Tạo tài khoản', '2024-05-15 14:35:57.49446', 8, 1);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (23, 'duyệt', '2024-05-18 20:17:41.686236', 3, 2);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (24, 'duyệt', '2024-05-18 20:18:07.863793', 6, 2);
INSERT INTO public.tutor_status_detail (id, context, create_date, tutor_id, status_id) VALUES (25, '{"TutorId":5,"Fullname":"Trần Thái Mỹ","Birth":"2002-01-01","Email":"giasu3@gmail.com","Phone":"0888888885","Gender":"M","Avatar":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-avatar%2Fba1fb6b8-9419-4ed8-816b-d69165f28995.jpg?alt=media&token=105b8cda-f120-4d1d-8982-cfab96a3590d","AddressDetail":"123 Nguyễn Hồng","SelectedProvinceId":79,"SelectedDistrictId":773,"IdentityCard":"888888885","FrontIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2F2ae69a72-529d-46f6-b115-80aa00084a0d.jpg?alt=media&token=dafbfb73-df16-4fe1-8e62-fb1b05ea29da","BackIdentityCard":"https://firebasestorage.googleapis.com/v0/b/giasuproject-imagestorage.appspot.com/o/user-identity_card%2Fecfe22cf-4211-42ea-a378-89af3934b8f9.jpg?alt=media&token=68439996-ea11-4ced-b0b0-ec282b1a4c24","College":"Đại Học Sư Phạm Kĩ Thuật","Area":"Cơ Khí","IsActive":true,"Additionalinfo":"Dạy ok","Academicyearfrom":2019,"Academicyearto":2023,"SelectedTutorTypeId":1,"FormatAddress":"","FormatTutorType":"Sinh viên","FormatTeachingArea":"","FormatGrades":"","FormatSessions":"","FormatSubjects":"","SelectedSubjectIds":[8],"SelectedSessionIds":[5],"SelectedDistricts":[771],"SelectedGradeIds":[6]}', '2024-05-18 20:21:28.010003', 5, 4);


--
-- TOC entry 5089 (class 0 OID 18968)
-- Dependencies: 252
-- Data for Name: tutor_subject; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (1, 1);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (1, 2);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (1, 3);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (1, 4);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (1, 5);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (1, 6);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (1, 7);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (1, 8);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (1, 9);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (1, 10);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (2, 10);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (3, 3);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (3, 5);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (3, 6);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (4, 10);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (5, 8);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (6, 7);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (7, 3);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (3, 10);
INSERT INTO public.tutor_subject (tutor_id, subject_id) VALUES (8, 3);


--
-- TOC entry 5090 (class 0 OID 18971)
-- Dependencies: 253
-- Data for Name: tutor_teaching_area; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (1, 568);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (1, 569);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (1, 570);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (1, 571);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (1, 572);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (1, 573);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (1, 574);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (1, 575);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (1, 576);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (2, 275);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (3, 5);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (3, 274);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (3, 281);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (4, 5);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (4, 274);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (5, 771);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (6, 275);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (7, 5);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (4, 7);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (4, 19);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (4, 279);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (3, 8);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (3, 279);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (8, 18);
INSERT INTO public.tutor_teaching_area (tutor_id, district_id) VALUES (5, 778);


--
-- TOC entry 5091 (class 0 OID 18974)
-- Dependencies: 254
-- Data for Name: tutor_type; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tutor_type (id, name, value) VALUES (1, 'Sinh viên', 1);
INSERT INTO public.tutor_type (id, name, value) VALUES (2, 'Giáo viên', 2);
INSERT INTO public.tutor_type (id, name, value) VALUES (3, 'Khác ', 3);


--
-- TOC entry 5119 (class 0 OID 0)
-- Dependencies: 216
-- Name: account_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.account_id_seq', 17, true);


--
-- TOC entry 5120 (class 0 OID 0)
-- Dependencies: 218
-- Name: customer_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.customer_id_seq', 3, true);


--
-- TOC entry 5121 (class 0 OID 0)
-- Dependencies: 220
-- Name: district_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.district_id_seq', 1, false);


--
-- TOC entry 5122 (class 0 OID 0)
-- Dependencies: 222
-- Name: employee_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employee_id_seq', 6, true);


--
-- TOC entry 5123 (class 0 OID 0)
-- Dependencies: 224
-- Name: grade_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.grade_id_seq', 20, true);


--
-- TOC entry 5124 (class 0 OID 0)
-- Dependencies: 226
-- Name: identitycard_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.identitycard_id_seq', 17, true);


--
-- TOC entry 5125 (class 0 OID 0)
-- Dependencies: 228
-- Name: province_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.province_id_seq', 1, false);


--
-- TOC entry 5126 (class 0 OID 0)
-- Dependencies: 230
-- Name: registerstatusdetail_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.registerstatusdetail_id_seq', 25, true);


--
-- TOC entry 5127 (class 0 OID 0)
-- Dependencies: 233
-- Name: requeststatus_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.requeststatus_id_seq', 1, true);


--
-- TOC entry 5128 (class 0 OID 0)
-- Dependencies: 235
-- Name: role_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.role_id_seq', 4, true);


--
-- TOC entry 5129 (class 0 OID 0)
-- Dependencies: 237
-- Name: sessiondate_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.sessiondate_id_seq', 27, true);


--
-- TOC entry 5130 (class 0 OID 0)
-- Dependencies: 239
-- Name: status_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.status_id_seq', 20, true);


--
-- TOC entry 5131 (class 0 OID 0)
-- Dependencies: 241
-- Name: statustype_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.statustype_id_seq', 4, true);


--
-- TOC entry 5132 (class 0 OID 0)
-- Dependencies: 243
-- Name: subject_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.subject_id_seq', 19, true);


--
-- TOC entry 5133 (class 0 OID 0)
-- Dependencies: 245
-- Name: transactionhistory_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.transactionhistory_id_seq', 6, true);


--
-- TOC entry 5134 (class 0 OID 0)
-- Dependencies: 249
-- Name: tutor_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tutor_id_seq', 8, true);


--
-- TOC entry 5135 (class 0 OID 0)
-- Dependencies: 255
-- Name: tutor_type_tutor_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tutor_type_tutor_id_seq', 3, true);


--
-- TOC entry 5136 (class 0 OID 0)
-- Dependencies: 256
-- Name: tutorrequestform_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tutorrequestform_id_seq', 19, true);


--
-- TOC entry 4779 (class 2606 OID 18998)
-- Name: account account_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account
    ADD CONSTRAINT account_email_key UNIQUE (email);


--
-- TOC entry 4781 (class 2606 OID 19000)
-- Name: account account_phone_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account
    ADD CONSTRAINT account_phone_key UNIQUE (phone);


--
-- TOC entry 4783 (class 2606 OID 19002)
-- Name: account account_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account
    ADD CONSTRAINT account_pkey PRIMARY KEY (id);


--
-- TOC entry 4785 (class 2606 OID 19004)
-- Name: customer customer_accountid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_accountid_key UNIQUE (account_id);


--
-- TOC entry 4787 (class 2606 OID 19006)
-- Name: customer customer_identityid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_identityid_key UNIQUE (identity_id);


--
-- TOC entry 4789 (class 2606 OID 19008)
-- Name: customer customer_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_pkey PRIMARY KEY (id);


--
-- TOC entry 4791 (class 2606 OID 19010)
-- Name: district district_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.district
    ADD CONSTRAINT district_name_key UNIQUE (name);


--
-- TOC entry 4793 (class 2606 OID 19012)
-- Name: district district_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.district
    ADD CONSTRAINT district_pkey PRIMARY KEY (id);


--
-- TOC entry 4795 (class 2606 OID 19014)
-- Name: employee employee_accountid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_accountid_key UNIQUE (account_id);


--
-- TOC entry 4797 (class 2606 OID 19016)
-- Name: employee employee_identityid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_identityid_key UNIQUE (identity_id);


--
-- TOC entry 4799 (class 2606 OID 19018)
-- Name: employee employee_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_pkey PRIMARY KEY (id);


--
-- TOC entry 4801 (class 2606 OID 19020)
-- Name: grade grade_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grade
    ADD CONSTRAINT grade_name_key UNIQUE (name);


--
-- TOC entry 4803 (class 2606 OID 19022)
-- Name: grade grade_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grade
    ADD CONSTRAINT grade_pkey PRIMARY KEY (id);


--
-- TOC entry 4805 (class 2606 OID 19024)
-- Name: grade grade_value_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.grade
    ADD CONSTRAINT grade_value_key UNIQUE (value);


--
-- TOC entry 4807 (class 2606 OID 19026)
-- Name: identity_card identitycard_identitynumber_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.identity_card
    ADD CONSTRAINT identitycard_identitynumber_key UNIQUE (identity_number);


--
-- TOC entry 4809 (class 2606 OID 19028)
-- Name: identity_card identitycard_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.identity_card
    ADD CONSTRAINT identitycard_pkey PRIMARY KEY (id);


--
-- TOC entry 4811 (class 2606 OID 19030)
-- Name: province province_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.province
    ADD CONSTRAINT province_pkey PRIMARY KEY (id);


--
-- TOC entry 4813 (class 2606 OID 19032)
-- Name: province province_provincename_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.province
    ADD CONSTRAINT province_provincename_key UNIQUE (name);


--
-- TOC entry 4815 (class 2606 OID 19034)
-- Name: tutor_status_detail registerstatusdetail_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_status_detail
    ADD CONSTRAINT registerstatusdetail_pkey PRIMARY KEY (id);


--
-- TOC entry 4817 (class 2606 OID 19036)
-- Name: request_status requeststatus_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_status
    ADD CONSTRAINT requeststatus_pkey PRIMARY KEY (id);


--
-- TOC entry 4821 (class 2606 OID 19038)
-- Name: role role_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role
    ADD CONSTRAINT role_name_key UNIQUE (name);


--
-- TOC entry 4823 (class 2606 OID 19040)
-- Name: role role_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.role
    ADD CONSTRAINT role_pkey PRIMARY KEY (id);


--
-- TOC entry 4825 (class 2606 OID 19042)
-- Name: session_date sessiondate_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.session_date
    ADD CONSTRAINT sessiondate_name_key UNIQUE (name);


--
-- TOC entry 4827 (class 2606 OID 19044)
-- Name: session_date sessiondate_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.session_date
    ADD CONSTRAINT sessiondate_pkey PRIMARY KEY (id);


--
-- TOC entry 4829 (class 2606 OID 19046)
-- Name: session_date sessiondate_value_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.session_date
    ADD CONSTRAINT sessiondate_value_key UNIQUE (value);


--
-- TOC entry 4831 (class 2606 OID 19048)
-- Name: status status_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status
    ADD CONSTRAINT status_pkey PRIMARY KEY (id);


--
-- TOC entry 4835 (class 2606 OID 19050)
-- Name: status_type statustype_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status_type
    ADD CONSTRAINT statustype_pkey PRIMARY KEY (id);


--
-- TOC entry 4837 (class 2606 OID 19052)
-- Name: status_type statustype_type_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status_type
    ADD CONSTRAINT statustype_type_key UNIQUE (type);


--
-- TOC entry 4839 (class 2606 OID 19054)
-- Name: subject subject_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subject
    ADD CONSTRAINT subject_name_key UNIQUE (name);


--
-- TOC entry 4841 (class 2606 OID 19056)
-- Name: subject subject_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subject
    ADD CONSTRAINT subject_pkey PRIMARY KEY (id);


--
-- TOC entry 4843 (class 2606 OID 19058)
-- Name: subject subject_value_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subject
    ADD CONSTRAINT subject_value_key UNIQUE (value);


--
-- TOC entry 4845 (class 2606 OID 19060)
-- Name: transaction_history transactionhistory_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history
    ADD CONSTRAINT transactionhistory_pkey PRIMARY KEY (id);


--
-- TOC entry 4847 (class 2606 OID 19062)
-- Name: tutor tutor_accountid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_accountid_key UNIQUE (account_id);


--
-- TOC entry 4849 (class 2606 OID 19064)
-- Name: tutor tutor_identityid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_identityid_key UNIQUE (identity_id);


--
-- TOC entry 4851 (class 2606 OID 19066)
-- Name: tutor tutor_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_pkey PRIMARY KEY (id);


--
-- TOC entry 4865 (class 2606 OID 19068)
-- Name: tutor_type tutor_type_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_type
    ADD CONSTRAINT tutor_type_name_key UNIQUE (name);


--
-- TOC entry 4867 (class 2606 OID 19070)
-- Name: tutor_type tutor_type_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_type
    ADD CONSTRAINT tutor_type_pkey PRIMARY KEY (id);


--
-- TOC entry 4869 (class 2606 OID 19072)
-- Name: tutor_type tutor_type_value_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_type
    ADD CONSTRAINT tutor_type_value_key UNIQUE (value);


--
-- TOC entry 4855 (class 2606 OID 19074)
-- Name: tutor_grade tutorgrade_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_grade
    ADD CONSTRAINT tutorgrade_pkey PRIMARY KEY (tutor_id, grade_id);


--
-- TOC entry 4853 (class 2606 OID 19076)
-- Name: tutor_apply_form tutorqueue_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_apply_form
    ADD CONSTRAINT tutorqueue_pkey PRIMARY KEY (tutor_request_id, tutor_id);


--
-- TOC entry 4819 (class 2606 OID 19078)
-- Name: request_tutor_form tutorrequestform_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_pkey PRIMARY KEY (id);


--
-- TOC entry 4857 (class 2606 OID 19080)
-- Name: tutor_request_session tutorrequestsession_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_request_session
    ADD CONSTRAINT tutorrequestsession_pkey PRIMARY KEY (tutor_request_id, session_id);


--
-- TOC entry 4859 (class 2606 OID 19082)
-- Name: tutor_session tutorsession_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_session
    ADD CONSTRAINT tutorsession_pkey PRIMARY KEY (tutor_id, session_id);


--
-- TOC entry 4861 (class 2606 OID 19084)
-- Name: tutor_subject tutorsubject_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_subject
    ADD CONSTRAINT tutorsubject_pkey PRIMARY KEY (tutor_id, subject_id);


--
-- TOC entry 4863 (class 2606 OID 19086)
-- Name: tutor_teaching_area tutorteachingareas_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_teaching_area
    ADD CONSTRAINT tutorteachingareas_pkey PRIMARY KEY (tutor_id, district_id);


--
-- TOC entry 4833 (class 2606 OID 19088)
-- Name: status uq_name_status_type_id; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status
    ADD CONSTRAINT uq_name_status_type_id UNIQUE (name, status_type_id);


--
-- TOC entry 4870 (class 2606 OID 19089)
-- Name: account account_roleid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account
    ADD CONSTRAINT account_roleid_fkey FOREIGN KEY (role_id) REFERENCES public.role(id);


--
-- TOC entry 4871 (class 2606 OID 19094)
-- Name: customer customer_accountid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_accountid_fkey FOREIGN KEY (account_id) REFERENCES public.account(id);


--
-- TOC entry 4872 (class 2606 OID 19099)
-- Name: customer customer_districtid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_districtid_fkey FOREIGN KEY (district_id) REFERENCES public.district(id);


--
-- TOC entry 4873 (class 2606 OID 19104)
-- Name: customer customer_identityid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_identityid_fkey FOREIGN KEY (identity_id) REFERENCES public.identity_card(id);


--
-- TOC entry 4874 (class 2606 OID 19109)
-- Name: district district_provinceid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.district
    ADD CONSTRAINT district_provinceid_fkey FOREIGN KEY (province_id) REFERENCES public.province(id);


--
-- TOC entry 4875 (class 2606 OID 19114)
-- Name: employee employee_accountid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_accountid_fkey FOREIGN KEY (account_id) REFERENCES public.account(id);


--
-- TOC entry 4876 (class 2606 OID 19119)
-- Name: employee employee_districtid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_districtid_fkey FOREIGN KEY (district_id) REFERENCES public.district(id);


--
-- TOC entry 4877 (class 2606 OID 19124)
-- Name: employee employee_identityid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee
    ADD CONSTRAINT employee_identityid_fkey FOREIGN KEY (identity_id) REFERENCES public.identity_card(id);


--
-- TOC entry 4891 (class 2606 OID 19129)
-- Name: tutor fk_tutor_status; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT fk_tutor_status FOREIGN KEY (status_id) REFERENCES public.status(id);


--
-- TOC entry 4878 (class 2606 OID 19134)
-- Name: tutor_status_detail registerstatusdetail_statusid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_status_detail
    ADD CONSTRAINT registerstatusdetail_statusid_fkey FOREIGN KEY (status_id) REFERENCES public.status(id);


--
-- TOC entry 4879 (class 2606 OID 19139)
-- Name: tutor_status_detail registerstatusdetail_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_status_detail
    ADD CONSTRAINT registerstatusdetail_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- TOC entry 4880 (class 2606 OID 19144)
-- Name: request_status requeststatus_tutorrequestid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_status
    ADD CONSTRAINT requeststatus_tutorrequestid_fkey FOREIGN KEY (tutor_request_id) REFERENCES public.request_tutor_form(id);


--
-- TOC entry 4886 (class 2606 OID 19149)
-- Name: status status_statustypeid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.status
    ADD CONSTRAINT status_statustypeid_fkey FOREIGN KEY (status_type_id) REFERENCES public.status_type(id);


--
-- TOC entry 4887 (class 2606 OID 19154)
-- Name: transaction_history transaction_history_status_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history
    ADD CONSTRAINT transaction_history_status_id_fkey FOREIGN KEY (status_id) REFERENCES public.status(id);


--
-- TOC entry 4888 (class 2606 OID 19159)
-- Name: transaction_history transaction_history_tutor_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history
    ADD CONSTRAINT transaction_history_tutor_id_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- TOC entry 4889 (class 2606 OID 19164)
-- Name: transaction_history transactionhistory_employeeid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history
    ADD CONSTRAINT transactionhistory_employeeid_fkey FOREIGN KEY (employee_id) REFERENCES public.employee(id);


--
-- TOC entry 4890 (class 2606 OID 19169)
-- Name: transaction_history transactionhistory_formid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.transaction_history
    ADD CONSTRAINT transactionhistory_formid_fkey FOREIGN KEY (form_id) REFERENCES public.request_tutor_form(id);


--
-- TOC entry 4892 (class 2606 OID 19174)
-- Name: tutor tutor_accountid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_accountid_fkey FOREIGN KEY (account_id) REFERENCES public.account(id);


--
-- TOC entry 4893 (class 2606 OID 19179)
-- Name: tutor tutor_districtid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_districtid_fkey FOREIGN KEY (district_id) REFERENCES public.district(id);


--
-- TOC entry 4894 (class 2606 OID 19184)
-- Name: tutor tutor_identityid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_identityid_fkey FOREIGN KEY (identity_id) REFERENCES public.identity_card(id);


--
-- TOC entry 4895 (class 2606 OID 19189)
-- Name: tutor tutor_tutor_type_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor
    ADD CONSTRAINT tutor_tutor_type_id_fkey FOREIGN KEY (tutor_type_id) REFERENCES public.tutor_type(id);


--
-- TOC entry 4899 (class 2606 OID 19194)
-- Name: tutor_grade tutorgrade_gradeid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_grade
    ADD CONSTRAINT tutorgrade_gradeid_fkey FOREIGN KEY (grade_id) REFERENCES public.grade(id);


--
-- TOC entry 4900 (class 2606 OID 19199)
-- Name: tutor_grade tutorgrade_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_grade
    ADD CONSTRAINT tutorgrade_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- TOC entry 4896 (class 2606 OID 19204)
-- Name: tutor_apply_form tutorqueue_statusid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_apply_form
    ADD CONSTRAINT tutorqueue_statusid_fkey FOREIGN KEY (status_id) REFERENCES public.status(id);


--
-- TOC entry 4897 (class 2606 OID 19209)
-- Name: tutor_apply_form tutorqueue_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_apply_form
    ADD CONSTRAINT tutorqueue_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- TOC entry 4898 (class 2606 OID 19214)
-- Name: tutor_apply_form tutorqueue_tutorrequestid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_apply_form
    ADD CONSTRAINT tutorqueue_tutorrequestid_fkey FOREIGN KEY (tutor_request_id) REFERENCES public.request_tutor_form(id);


--
-- TOC entry 4881 (class 2606 OID 19219)
-- Name: request_tutor_form tutorrequestform_customerid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_customerid_fkey FOREIGN KEY (customer_id) REFERENCES public.customer(id);


--
-- TOC entry 4882 (class 2606 OID 19224)
-- Name: request_tutor_form tutorrequestform_districtid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_districtid_fkey FOREIGN KEY (district_id) REFERENCES public.district(id);


--
-- TOC entry 4883 (class 2606 OID 19229)
-- Name: request_tutor_form tutorrequestform_gradeid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_gradeid_fkey FOREIGN KEY (grade_id) REFERENCES public.grade(id);


--
-- TOC entry 4884 (class 2606 OID 19234)
-- Name: request_tutor_form tutorrequestform_statusid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_statusid_fkey FOREIGN KEY (status_id) REFERENCES public.status(id);


--
-- TOC entry 4885 (class 2606 OID 19239)
-- Name: request_tutor_form tutorrequestform_subjectid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.request_tutor_form
    ADD CONSTRAINT tutorrequestform_subjectid_fkey FOREIGN KEY (subject_id) REFERENCES public.subject(id);


--
-- TOC entry 4901 (class 2606 OID 19244)
-- Name: tutor_request_session tutorrequestsession_sessionid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_request_session
    ADD CONSTRAINT tutorrequestsession_sessionid_fkey FOREIGN KEY (session_id) REFERENCES public.session_date(id);


--
-- TOC entry 4902 (class 2606 OID 19249)
-- Name: tutor_request_session tutorrequestsession_tutorrequestid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_request_session
    ADD CONSTRAINT tutorrequestsession_tutorrequestid_fkey FOREIGN KEY (tutor_request_id) REFERENCES public.request_tutor_form(id);


--
-- TOC entry 4903 (class 2606 OID 19254)
-- Name: tutor_session tutorsession_sessionid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_session
    ADD CONSTRAINT tutorsession_sessionid_fkey FOREIGN KEY (session_id) REFERENCES public.session_date(id);


--
-- TOC entry 4904 (class 2606 OID 19259)
-- Name: tutor_session tutorsession_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_session
    ADD CONSTRAINT tutorsession_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- TOC entry 4905 (class 2606 OID 19264)
-- Name: tutor_subject tutorsubject_subjectid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_subject
    ADD CONSTRAINT tutorsubject_subjectid_fkey FOREIGN KEY (subject_id) REFERENCES public.subject(id);


--
-- TOC entry 4906 (class 2606 OID 19269)
-- Name: tutor_subject tutorsubject_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_subject
    ADD CONSTRAINT tutorsubject_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- TOC entry 4907 (class 2606 OID 19274)
-- Name: tutor_teaching_area tutorteachingareas_districtid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_teaching_area
    ADD CONSTRAINT tutorteachingareas_districtid_fkey FOREIGN KEY (district_id) REFERENCES public.district(id);


--
-- TOC entry 4908 (class 2606 OID 19279)
-- Name: tutor_teaching_area tutorteachingareas_tutorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tutor_teaching_area
    ADD CONSTRAINT tutorteachingareas_tutorid_fkey FOREIGN KEY (tutor_id) REFERENCES public.tutor(id);


--
-- TOC entry 5100 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;


-- Completed on 2024-05-18 20:51:42

--
-- PostgreSQL database dump complete
--

