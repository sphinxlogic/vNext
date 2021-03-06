/*
  Copyright (c) 2017 Microsoft Corporation
  Author: Lev Nachmanson
*/

#include "math/lp/int_solver.h"
#include "math/lp/lar_solver.h"
#include "math/lp/lp_utils.h"
#include <utility>
#include "math/lp/monic.h"
#include "math/lp/gomory.h"
namespace lp {


std::ostream& int_solver::display_inf_rows(std::ostream& out) const {
    unsigned num = m_lar_solver->A_r().column_count();
    for (unsigned v = 0; v < num; v++) {
        if (column_is_int(v) && !get_value(v).is_int()) {
            display_column(out, v);
        }
    }
    
    num = 0;
    for (unsigned i = 0; i < m_lar_solver->A_r().row_count(); i++) {
        unsigned j = m_lar_solver->m_mpq_lar_core_solver.m_r_basis[i];
        if (column_is_int_inf(j)) {
            num++;
            m_lar_solver->print_row(m_lar_solver->A_r().m_rows[i], out);
            out << "\n";
        }
    }
    out << "num of int infeasible: " << num << "\n";
    return out;
}

bool int_solver::has_inf_int() const {
    return m_lar_solver->has_inf_int();
}

int int_solver::find_inf_int_base_column() {
    unsigned inf_int_count = 0;
    int j = find_inf_int_boxed_base_column_with_smallest_range(inf_int_count);
    if (j != -1) {
        return j;
    }
    if (inf_int_count == 0)
        return -1;
    unsigned k = random() % inf_int_count;
    return get_kth_inf_int(k);
}

int int_solver::get_kth_inf_int(unsigned k) const {
    for (unsigned j : m_lar_solver->r_basis()) 
        if (column_is_int_inf(j) && k-- == 0)
            return j;
    lp_assert(false);
    return -1;
}

int int_solver::find_inf_int_nbasis_column() const {
    for (unsigned j : m_lar_solver->r_nbasis())
        if (!column_is_int_inf(j)) {
            return j;    
        }
    return -1; 
}

int int_solver::find_inf_int_boxed_base_column_with_smallest_range(unsigned & inf_int_count) {
    inf_int_count = 0;
    int result = -1;
    mpq range;
    mpq new_range;
    mpq small_range_thresold(1024);
    unsigned n = 0;
    lar_core_solver & lcs = m_lar_solver->m_mpq_lar_core_solver;

    for (unsigned j : m_lar_solver->r_basis()) {
        if (!column_is_int_inf(j))
            continue;
        inf_int_count++;
        if (!is_boxed(j))
            continue;
        lp_assert(!is_fixed(j));
        new_range  = lcs.m_r_upper_bounds()[j].x - lcs.m_r_lower_bounds()[j].x;
        if (new_range > small_range_thresold) 
            continue;
        if (result == -1 || new_range < range) {
            result = j;
            range  = new_range;
            n      = 1;
        }
        else if (new_range == range && settings().random_next() % (++n) == 0) {
            lp_assert(n > 1);
            result = j;
        }
    }
    return result;
    
}

bool int_solver::is_gomory_cut_target(const row_strip<mpq>& row) {
    // All non base variables must be at their bounds and assigned to rationals (that is, infinitesimals are not allowed).
    unsigned j;
    for (const auto & p : row) {
        j = p.var();
        if (!is_base(j) && (!at_bound(j) || !is_zero(get_value(j).y))) {
            TRACE("gomory_cut", tout << "row is not gomory cut target:\n";
                  display_column(tout, j);
                  tout << "infinitesimal: " << !is_zero(get_value(j).y) << "\n";);
            return false;
        }
    }
    return true;
}



constraint_index int_solver::column_upper_bound_constraint(unsigned j) const {
    return m_lar_solver->get_column_upper_bound_witness(j);
}

bool int_solver::current_solution_is_inf_on_cut() const {
    const auto & x = m_lar_solver->m_mpq_lar_core_solver.m_r_x;
    impq v = m_t.apply(x);
    mpq sign = m_upper ? one_of_type<mpq>()  : -one_of_type<mpq>();
    CTRACE("current_solution_is_inf_on_cut", v * sign <= impq(m_k) * sign,
           tout << "m_upper = " << m_upper << std::endl;
           tout << "v = " << v << ", k = " << m_k << std::endl;
          );
    return v * sign > impq(m_k) * sign;
}

constraint_index int_solver::column_lower_bound_constraint(unsigned j) const {
    return m_lar_solver->get_column_lower_bound_witness(j);
}

lia_move int_solver::mk_gomory_cut( unsigned inf_col, const row_strip<mpq> & row) {
    lp_assert(column_is_int_inf(inf_col));
    gomory gc(m_t, m_k, m_ex, inf_col, row, *this);
    return gc.create_cut();
}

lia_move int_solver::proceed_with_gomory_cut(unsigned j) {
    const row_strip<mpq>& row = m_lar_solver->get_row(row_of_basic_column(j));

    SASSERT(m_lar_solver->row_is_correct(row_of_basic_column(j)));
    
    if (!is_gomory_cut_target(row)) 
        return create_branch_on_column(j);

    m_upper = true;
    return mk_gomory_cut(j, row);
}


unsigned int_solver::row_of_basic_column(unsigned j) const {
    return m_lar_solver->row_of_basic_column(j);
}


// this will allow to enable and disable tracking of the pivot rows
struct check_return_helper {
    lar_solver *     m_lar_solver;
    bool             m_track_pivoted_rows;
    check_return_helper(lar_solver* ls) :
        m_lar_solver(ls),
        m_track_pivoted_rows(ls->get_track_pivoted_rows())
    {
        TRACE("pivoted_rows", tout << "pivoted rows = " << ls->m_mpq_lar_core_solver.m_r_solver.m_pivoted_rows->size() << std::endl;);
        m_lar_solver->set_track_pivoted_rows(false);
    }
    ~check_return_helper() {
        TRACE("pivoted_rows", tout << "pivoted rows = " << m_lar_solver->m_mpq_lar_core_solver.m_r_solver.m_pivoted_rows->size() << std::endl;);
        m_lar_solver->set_track_pivoted_rows(m_track_pivoted_rows);
    }
};



impq int_solver::get_cube_delta_for_term(const lar_term& t) const {
    if (t.size() == 2) {
        bool seen_minus = false;
        bool seen_plus = false;
        for(const auto & p : t) {
            if (!column_is_int(p.var()))
                goto usual_delta;
            const mpq & c = p.coeff();
            if (c == one_of_type<mpq>()) {
                seen_plus = true;
            } else if (c == -one_of_type<mpq>()) {
                seen_minus = true;
            } else {
                goto usual_delta;
            }
        }
        if (seen_minus && seen_plus)
            return zero_of_type<impq>();
        return impq(0, 1);
    }
 usual_delta:
    mpq delta = zero_of_type<mpq>();
    for (const auto & p : t)
        if (column_is_int(p.var()))
            delta += abs(p.coeff());
    
    delta *= mpq(1, 2);
    return impq(delta);
}

bool int_solver::tighten_term_for_cube(unsigned i) {
    unsigned ti = i + m_lar_solver->terms_start_index();
    if (!m_lar_solver->term_is_used_as_row(ti))
        return true;
    const lar_term* t = m_lar_solver->terms()[i];
    impq delta = get_cube_delta_for_term(*t);
    TRACE("cube", m_lar_solver->print_term_as_indices(*t, tout); tout << ", delta = " << delta;);
    if (is_zero(delta))
        return true;
    return m_lar_solver->tighten_term_bounds_by_delta(i, delta);
}

bool int_solver::tighten_terms_for_cube() {
    for (unsigned i = 0; i < m_lar_solver->terms().size(); i++)
        if (!tighten_term_for_cube(i)) {
            TRACE("cube", tout << "cannot tighten";);
            return false;
        }
    return true;
}

bool int_solver::should_find_cube() {
    return m_number_of_calls % settings().m_int_find_cube_period == 0;
}

lia_move int_solver::find_cube() {
    if (!should_find_cube())
        return lia_move::undef;
    
    settings().stats().m_cube_calls++;
    TRACE("cube",
          for (unsigned j = 0; j < m_lar_solver->A_r().column_count(); j++)
              display_column(tout, j);
          tout << m_lar_solver->constraints();
          );
    
    m_lar_solver->push();
    if (!tighten_terms_for_cube()) {
        m_lar_solver->pop();
        return lia_move::undef;
    }

    lp_status st = m_lar_solver->find_feasible_solution();
    if (st != lp_status::FEASIBLE && st != lp_status::OPTIMAL) {
        TRACE("cube", tout << "cannot find a feasiblie solution";);
        m_lar_solver->pop();
        m_lar_solver->move_non_basic_columns_to_bounds();
        find_feasible_solution();
        // it can happen that we found an integer solution here
        return !m_lar_solver->r_basis_has_inf_int()? lia_move::sat: lia_move::undef;
    }
    m_lar_solver->pop();
    m_lar_solver->round_to_integer_solution();
    m_lar_solver->set_status(lp_status::FEASIBLE);
    lp_assert(settings().get_cancel_flag() || is_feasible());
    TRACE("cube", tout << "success";);
    settings().stats().m_cube_success++;
    return lia_move::sat;
}

void int_solver::find_feasible_solution() {
    m_lar_solver->find_feasible_solution();
    lp_assert(lp_status::OPTIMAL == m_lar_solver->get_status() || lp_status::FEASIBLE == m_lar_solver->get_status());
}

bool int_solver::should_run_gcd_test() {
    return settings().m_int_run_gcd_test;    
}

lia_move int_solver::run_gcd_test() {
    if (should_run_gcd_test()) {
        settings().stats().m_gcd_calls++;
        TRACE("int_solver", tout << "gcd-test " << settings().stats().m_gcd_calls << "\n";);
        if (!gcd_test()) {
            settings().stats().m_gcd_conflicts++;
            TRACE("gcd_test", tout << "gcd conflict\n";);
            return lia_move::conflict;
        }
    }
    return lia_move::undef;
}

bool int_solver::should_gomory_cut() {
    return m_number_of_calls % settings().m_int_gomory_cut_period == 0;
}

lia_move int_solver::gomory_cut() {
    TRACE("int_solver", tout << "gomory " << m_number_of_calls << "\n";);
    if (!should_gomory_cut())
        return lia_move::undef;

    if (m_lar_solver->move_non_basic_columns_to_bounds()) {
        lp_status st = m_lar_solver->find_feasible_solution();
        (void)st;
        lp_assert(st == lp_status::FEASIBLE || st == lp_status::OPTIMAL);
    }
        
    int j = find_inf_int_base_column(); 
    if (j == -1) {
        j = find_inf_int_nbasis_column();
        return j == -1? lia_move::sat : create_branch_on_column(j);
    }
    return proceed_with_gomory_cut(j);
}

void int_solver::try_add_term_to_A_for_hnf(unsigned i) {
    mpq rs;
    const lar_term* t = m_lar_solver->terms()[i];
    constraint_index ci;
    bool upper_bound;
    if (!hnf_cutter_is_full() && m_lar_solver->get_equality_and_right_side_for_term_on_current_x(i, rs, ci, upper_bound)) {
        m_hnf_cutter.add_term(t, rs, ci, upper_bound);
    }
}

bool int_solver::hnf_cutter_is_full() const {
    return
        m_hnf_cutter.terms_count() >= settings().limit_on_rows_for_hnf_cutter
                                     ||
        m_hnf_cutter.vars().size() >= settings().limit_on_columns_for_hnf_cutter;
}

lp_settings& int_solver::settings() {
    return m_lar_solver->settings();
}

const lp_settings& int_solver::settings() const {
    return m_lar_solver->settings();
}

bool int_solver::hnf_has_var_with_non_integral_value() const {
    for (unsigned j : m_hnf_cutter.vars())
        if (!get_value(j).is_int())
            return true;
    return false;
}

bool int_solver::init_terms_for_hnf_cut() {
    m_hnf_cutter.clear();
    for (unsigned i = 0; i < m_lar_solver->terms().size() && !hnf_cutter_is_full(); i++) {
        try_add_term_to_A_for_hnf(i);
    }
    return hnf_has_var_with_non_integral_value();
}

lia_move int_solver::make_hnf_cut() {
    if (!init_terms_for_hnf_cut()) {
        return lia_move::undef;
    }
    settings().stats().m_hnf_cutter_calls++;
    TRACE("hnf_cut", tout << "settings().stats().m_hnf_cutter_calls = " << settings().stats().m_hnf_cutter_calls << "\n";
          for (unsigned i : m_hnf_cutter.constraints_for_explanation()) {
              m_lar_solver->constraints().display(tout, i);
          }              
          tout << m_lar_solver->constraints();
          );
#ifdef Z3DEBUG
    vector<mpq> x0 = m_hnf_cutter.transform_to_local_columns(m_lar_solver->m_mpq_lar_core_solver.m_r_x);
#else
    vector<mpq> x0;
#endif
    lia_move r =  m_hnf_cutter.create_cut(m_t, m_k, m_ex, m_upper, x0);

    if (r == lia_move::cut) {      
        TRACE("hnf_cut",
              m_lar_solver->print_term(m_t, tout << "cut:"); 
              tout << " <= " << m_k << std::endl;
              for (unsigned i : m_hnf_cutter.constraints_for_explanation()) {
                  m_lar_solver->constraints().display(tout, i);
              }              
              );
        lp_assert(current_solution_is_inf_on_cut());
        settings().stats().m_hnf_cuts++;
        m_ex->clear();        
        for (unsigned i : m_hnf_cutter.constraints_for_explanation()) {
             m_ex->push_justification(i);
        }
    } 
    return r;
}

bool int_solver::should_hnf_cut() {
    return settings().m_enable_hnf && m_number_of_calls % m_hnf_cut_period == 0;
}

lia_move int_solver::hnf_cut() {
    lia_move r = lia_move::undef;
    if (should_hnf_cut()) {
        r = make_hnf_cut();
        if (r == lia_move::undef) {
            m_hnf_cut_period *= 2;
        }
        else {
            m_hnf_cut_period = settings().hnf_cut_period();
        }
    }
    return r;
}

lia_move int_solver::check(lp::explanation * e) {
    SASSERT(m_lar_solver->ax_is_correct());
    if (!has_inf_int()) return lia_move::sat;

#define CHECK_RET(fn)                                                   \
    r = fn;                                                             \
    if (r != lia_move::undef) { TRACE("int_solver", tout << #fn << "\n";); return r; }

    m_t.clear();
    m_k.reset();
    m_ex = e;
    m_ex->clear();
    m_upper = false;
    lia_move r;

    CHECK_RET(run_gcd_test());

    check_return_helper pc(m_lar_solver);

    if (settings().m_int_pivot_fixed_vars_from_basis)
        m_lar_solver->pivot_fixed_vars_from_basis();

    CHECK_RET(patch_nbasic_columns());
    ++m_number_of_calls;
    CHECK_RET(find_cube());        
    CHECK_RET(hnf_cut());    
    CHECK_RET(gomory_cut());
    CHECK_RET(branch_or_sat());
    return r;
}

lia_move int_solver::branch_or_sat() {
    int j = find_any_inf_int_column_basis_first();
    return j == -1? lia_move::sat : create_branch_on_column(j);
}

int int_solver::find_any_inf_int_column_basis_first() {
   int j = find_inf_int_base_column();
   return j != -1 ? j : find_inf_int_nbasis_column();
}

void int_solver::set_value_for_nbasic_column_ignore_old_values(unsigned j, const impq & new_val) {
    lp_assert(!is_base(j));
    auto & x = m_lar_solver->m_mpq_lar_core_solver.m_r_x[j];
    auto delta = new_val - x;
    x = new_val;
    TRACE("int_solver", tout << "x[" << j << "] = " << x << "\n";);
    m_lar_solver->change_basic_columns_dependend_on_a_given_nb_column(j, delta);
}

void int_solver::patch_nbasic_column(unsigned j) {
    auto & lcs = m_lar_solver->m_mpq_lar_core_solver; 
    impq & val = lcs.m_r_x[j];
    bool inf_l, inf_u;
    impq l, u;
    mpq m;
    if (!get_freedom_interval_for_column(j, inf_l, l, inf_u, u, m)) {
        return;
    }
    bool m_is_one = m.is_one();
    bool val_is_int = value_is_int(j);

    // check whether value of j is already a multiple of m.
    if (val_is_int && (m_is_one || (val.x / m).is_int())) {
        return;
    }
    TRACE("patch_int",
          tout << "TARGET j" << j << " -> [";
          if (inf_l) tout << "-oo"; else tout << l;
          tout << ", ";
          if (inf_u) tout << "oo"; else tout << u;
          tout << "]";
          tout << ", m: " << m << ", val: " << val << ", is_int: " << m_lar_solver->column_is_int(j) << "\n";);
    if (!inf_l) {
        l = impq(m_is_one ? ceil(l) : m * ceil(l / m));
        if (inf_u || l <= u) {
            TRACE("patch_int",    tout << "patching with l: " << l << '\n';);
            m_lar_solver->set_value_for_nbasic_column(j, l);
        }
        else {
            TRACE("patch_int", tout << "not patching " << l << "\n";);
        }
    }
    else if (!inf_u) {
        u = impq(m_is_one ? floor(u) : m * floor(u / m));
        m_lar_solver->set_value_for_nbasic_column(j, u);
        TRACE("patch_int", tout << "patching with u: " << u << '\n';);
    }
    else {
        m_lar_solver->set_value_for_nbasic_column(j, impq(0));
        TRACE("patch_int", tout << "patching with 0\n";);
    }
}

lia_move int_solver::patch_nbasic_columns() {
    settings().stats().m_patches++;
    lp_assert(is_feasible());
    for (unsigned j : m_lar_solver->m_mpq_lar_core_solver.m_r_nbasis) {
        patch_nbasic_column(j);
    }
    lp_assert(is_feasible());
    if (!has_inf_int()) {
        settings().stats().m_patches_success++;
        return lia_move::sat;
    }
    return lia_move::undef;
}

mpq get_denominators_lcm(const row_strip<mpq> & row) {
    mpq r(1);
    for  (auto & c : row) {
        r = lcm(r, denominator(c.coeff()));
    }
    return r;
}
    
bool int_solver::gcd_test_for_row(static_matrix<mpq, numeric_pair<mpq>> & A, unsigned i) {
    auto const& row = A.m_rows[i];
    auto & lcs = m_lar_solver->m_mpq_lar_core_solver;
    unsigned basic_var = lcs.m_r_basis[i];

    if (!column_is_int(basic_var) || get_value(basic_var).is_int())
        return true;
    mpq lcm_den = get_denominators_lcm(row);
    mpq consts(0);
    mpq gcds(0);
    mpq least_coeff(0);
    bool    least_coeff_is_bounded = false;
    unsigned j;
    for (auto &c : A.m_rows[i]) {
        j = c.var();
        const mpq& a = c.coeff();
        if (m_lar_solver->column_is_fixed(j)) {
            mpq aux = lcm_den * a;
            consts += aux * m_lar_solver->column_lower_bound(j).x;
        }
        else if (m_lar_solver->column_is_real(j)) {
            return true;
        }
        else if (gcds.is_zero()) {
            gcds = abs(lcm_den * a);
            least_coeff = gcds;
            least_coeff_is_bounded = m_lar_solver->column_is_bounded(j);
        }
        else {
            mpq aux = abs(lcm_den * a);
            gcds = gcd(gcds, aux);
            if (aux < least_coeff) {
                least_coeff = aux;
                least_coeff_is_bounded = m_lar_solver->column_is_bounded(j);
            }
            else if (least_coeff_is_bounded && aux == least_coeff) {
                least_coeff_is_bounded = m_lar_solver->column_is_bounded(j);
            }
        }
        SASSERT(gcds.is_int());
        SASSERT(least_coeff.is_int());
        TRACE("gcd_test_bug", tout << "coeff: " << a << ", gcds: " << gcds 
              << " least_coeff: " << least_coeff << " consts: " << consts << "\n";);
        
    }
    
    if (gcds.is_zero()) {
        // All variables are fixed.
        // This theory guarantees that the assignment satisfies each row, and
        // fixed integer variables are assigned to integer values.
        return true;
    }
        
    if (!(consts / gcds).is_int()) {
        TRACE("gcd_test", tout << "row failed the GCD test:\n"; display_row_info(tout, i););
        fill_explanation_from_fixed_columns(A.m_rows[i]);
        return false;
    }
        
    if (least_coeff.is_one() && !least_coeff_is_bounded) {
        SASSERT(gcds.is_one());
        return true;
    }
        
    if (least_coeff_is_bounded) {
        return ext_gcd_test(A.m_rows[i], least_coeff, lcm_den, consts);
    }
    return true;
}


void int_solver::add_to_explanation_from_fixed_or_boxed_column(unsigned j) {
    constraint_index lc, uc;
    m_lar_solver->get_bound_constraint_witnesses_for_column(j, lc, uc);
    m_ex->push_justification(lc);
    m_ex->push_justification(uc);
}
void int_solver::fill_explanation_from_fixed_columns(const row_strip<mpq> & row) {
    for (const auto & c : row) {
        if (!m_lar_solver->column_is_fixed(c.var()))
            continue;
        add_to_explanation_from_fixed_or_boxed_column(c.var());
    }
}
    
bool int_solver::gcd_test() {
    auto & A = m_lar_solver->A_r(); // getting the matrix
    for (unsigned i = 0; i < A.row_count(); i++)
        if (!gcd_test_for_row(A, i)) 
            return false;        
    return true;
}

bool int_solver::ext_gcd_test(const row_strip<mpq> & row,
                              mpq const & least_coeff, 
                              mpq const & lcm_den,
                              mpq const & consts) {
    TRACE("ext_gcd_test", tout << "row = "; m_lar_solver->print_row(row, tout););
    mpq gcds(0);
    mpq l(consts);
    mpq u(consts);

    mpq a;
    unsigned j;
    for (const auto & c : row) {
        j = c.var();
        TRACE("ext_gcd_test", tout << "col = "; m_lar_solver->m_mpq_lar_core_solver.m_r_solver.print_column_bound_info(j, tout););
        const mpq & a = c.coeff();
        if (m_lar_solver->column_is_fixed(j))
            continue;
        SASSERT(!m_lar_solver->column_is_real(j));
        mpq ncoeff = lcm_den * a;
        SASSERT(ncoeff.is_int());
        mpq abs_ncoeff = abs(ncoeff);
        if (abs_ncoeff == least_coeff) {
            SASSERT(m_lar_solver->column_is_bounded(j));
            if (ncoeff.is_pos()) {
                // l += ncoeff * m_lar_solver->column_lower_bound(j).x;
                l.addmul(ncoeff, m_lar_solver->column_lower_bound(j).x);
                // u += ncoeff * m_lar_solver->column_upper_bound(j).x;
                u.addmul(ncoeff, m_lar_solver->column_upper_bound(j).x);
            }
            else {
                // l += ncoeff * upper_bound(j).get_rational();
                l.addmul(ncoeff, m_lar_solver->column_upper_bound(j).x);
                // u += ncoeff * lower_bound(j).get_rational();
                u.addmul(ncoeff, m_lar_solver->column_lower_bound(j).x);
            }
            add_to_explanation_from_fixed_or_boxed_column(j);
        }
        else if (gcds.is_zero()) {
            gcds = abs_ncoeff; 
        }
        else {
            gcds = gcd(gcds, abs_ncoeff);
        }
        SASSERT(gcds.is_int());
    }
        
    if (gcds.is_zero()) {
        return true;
    }
        
    mpq l1 = ceil(l/gcds);
    mpq u1 = floor(u/gcds);
        
    if (u1 < l1) {
        fill_explanation_from_fixed_columns(row);
        return false;
    }
        
    return true;

}
/*
linear_combination_iterator<mpq> * int_solver::get_column_iterator(unsigned j) {
    if (m_lar_solver->use_tableau())
        return new iterator_on_column<mpq, impq>(m_lar_solver->A_r().m_columns[j], m_lar_solver->A_r());
    return new iterator_on_indexed_vector<mpq>(m_lar_solver->get_column_in_lu_mode(j));
}
*/


int_solver::int_solver(lar_solver* lar_slv) :
    m_lar_solver(lar_slv),
    m_number_of_calls(0),
    m_hnf_cutter(settings()),
    m_hnf_cut_period(settings().hnf_cut_period()) {
    m_lar_solver->set_int_solver(this);
}


bool int_solver::has_lower(unsigned j) const {
    switch (m_lar_solver->m_mpq_lar_core_solver.m_column_types()[j]) {
    case column_type::fixed:
    case column_type::boxed:
    case column_type::lower_bound:
        return true;
    default:
        return false;
    }
}

bool int_solver::has_upper(unsigned j) const {
    switch (m_lar_solver->m_mpq_lar_core_solver.m_column_types()[j]) {
    case column_type::fixed:
    case column_type::boxed:
    case column_type::upper_bound:
        return true;
    default:
        return false;
    }
}


static void set_lower(impq & l, bool & inf_l, impq const & v ) {
    if (inf_l || v > l) {
        l = v;
        inf_l = false;
    }
}


static void set_upper(impq & u, bool & inf_u, impq const & v) {
    if (inf_u || v < u) {
        u = v;
        inf_u = false;
    }
}

bool int_solver::get_freedom_interval_for_column(unsigned j, bool & inf_l, impq & l, bool & inf_u, impq & u, mpq & m) {
    auto & lcs = m_lar_solver->m_mpq_lar_core_solver;
    if (lcs.m_r_heading[j] >= 0) // the basic var 
        return false;

    TRACE("random_update", display_column(tout, j) << ", is_int = " << column_is_int(j) << "\n";);
    impq const & xj = get_value(j);
    
    inf_l = true;
    inf_u = true;
    l = u = zero_of_type<impq>();
    m = mpq(1);

    if (has_lower(j)) {
        set_lower(l, inf_l, lower_bound(j) - xj);
    }
    if (has_upper(j)) {
        set_upper(u, inf_u, upper_bound(j) - xj);
    }

    unsigned row_index;
    lp_assert(settings().use_tableau());
    const auto & A = m_lar_solver->A_r();
    unsigned rounds = 0;
    for (const auto &c : A.column(j)) {
        row_index = c.var();
        const mpq & a = c.coeff();        
        unsigned i = lcs.m_r_basis[row_index];
        TRACE("random_update", tout << "i = " << i << ", a = " << a << "\n";); 
        if (column_is_int(i) && !a.is_int())
            m = lcm(m, denominator(a));
    }
    TRACE("random_update", tout <<  "m = " << m << "\n";);
    
    for (const auto &c : A.column(j)) {
        if (!inf_l && !inf_u && l >= u) break;                
        row_index = c.var();
        const mpq & a = c.coeff();        
        unsigned i = lcs.m_r_basis[row_index];
        impq const & xi = get_value(i);

#define SET_BOUND(_fn_, a, b, x, y, z)                                  \
        if (x.is_one())                                                 \
            _fn_(a, b, y - z);                                          \
        else if (x.is_minus_one())                                      \
            _fn_(a, b, z - y);                                          \
        else if (z == y)                                                \
            _fn_(a, b, zero_of_type<impq>());                           \
        else                                                            \
            {  _fn_(a, b, (y - z)/x);  }   \

        
        if (a.is_neg()) {
            if (has_lower(i)) {
                SET_BOUND(set_lower, l, inf_l, a, xi, lcs.m_r_lower_bounds()[i]);
            }
            if (has_upper(i)) {
                SET_BOUND(set_upper, u, inf_u, a, xi, lcs.m_r_upper_bounds()[i]);
            }
        }
        else {
            if (has_upper(i)) {
                SET_BOUND(set_lower, l, inf_l, a, xi, lcs.m_r_upper_bounds()[i]);
            }
            if (has_lower(i)) {
                SET_BOUND(set_upper, u, inf_u, a, xi, lcs.m_r_lower_bounds()[i]);
            }
        }
        ++rounds;
    }

    l += xj;
    u += xj;

    TRACE("freedom_interval",
          tout << "freedom variable for:\n";
          tout << m_lar_solver->get_variable_name(j);
          tout << "[";
          if (inf_l) tout << "-oo"; else tout << l;
          tout << "; ";
          if (inf_u) tout << "oo";  else tout << u;
          tout << "]\n";
          tout << "val = " << get_value(j) << "\n";
          tout << "return " << (inf_l || inf_u || l <= u);
          );
    return (inf_l || inf_u || l <= u);
}

bool int_solver::column_is_int(unsigned j) const {
    return m_lar_solver->column_is_int(j);
}

bool int_solver::is_real(unsigned j) const {
    return !column_is_int(j);
}

bool int_solver::value_is_int(unsigned j) const {
    return m_lar_solver->column_value_is_int(j);
}    

bool int_solver::is_feasible() const {
    auto & lcs = m_lar_solver->m_mpq_lar_core_solver;
    lp_assert(
        lcs.m_r_solver.calc_current_x_is_feasible_include_non_basis() ==
        lcs.m_r_solver.current_x_is_feasible());
    return lcs.m_r_solver.current_x_is_feasible();
}
const impq & int_solver::get_value(unsigned j) const {
    return m_lar_solver->m_mpq_lar_core_solver.m_r_x[j];
}

std::ostream& int_solver::display_column(std::ostream & out, unsigned j) const {
    m_lar_solver->m_mpq_lar_core_solver.m_r_solver.print_column_info(j, out);
    return out;
}

bool int_solver::column_is_int_inf(unsigned j) const {
    return column_is_int(j) && (!value_is_int(j));
}

bool int_solver::is_base(unsigned j) const {
    return m_lar_solver->m_mpq_lar_core_solver.m_r_heading[j] >= 0;
}

bool int_solver::is_boxed(unsigned j) const {
    return m_lar_solver->m_mpq_lar_core_solver.m_column_types[j] == column_type::boxed;
}

bool int_solver::is_fixed(unsigned j) const {
    return m_lar_solver->m_mpq_lar_core_solver.m_column_types[j] == column_type::fixed;
}

bool int_solver::is_free(unsigned j) const {
    return m_lar_solver->m_mpq_lar_core_solver.m_column_types[j] == column_type::free_column;
}

bool int_solver::at_bound(unsigned j) const {
    auto & mpq_solver = m_lar_solver->m_mpq_lar_core_solver.m_r_solver;
    switch (mpq_solver.m_column_types[j] ) {
    case column_type::fixed:
    case column_type::boxed:
        return
            mpq_solver.m_lower_bounds[j] == get_value(j) ||
            mpq_solver.m_upper_bounds[j] == get_value(j);
    case column_type::lower_bound:
        return mpq_solver.m_lower_bounds[j] == get_value(j);
    case column_type::upper_bound:
        return  mpq_solver.m_upper_bounds[j] == get_value(j);
    default:
        return false;
    }
}

bool int_solver::at_lower(unsigned j) const {
    auto & mpq_solver = m_lar_solver->m_mpq_lar_core_solver.m_r_solver;
    switch (mpq_solver.m_column_types[j] ) {
    case column_type::fixed:
    case column_type::boxed:
    case column_type::lower_bound:
        return mpq_solver.m_lower_bounds[j] == get_value(j);
    default:
        return false;
    }
}

bool int_solver::at_upper(unsigned j) const {
    auto & mpq_solver = m_lar_solver->m_mpq_lar_core_solver.m_r_solver;
    switch (mpq_solver.m_column_types[j] ) {
    case column_type::fixed:
    case column_type::boxed:
    case column_type::upper_bound:
        return mpq_solver.m_upper_bounds[j] == get_value(j);
    default:
        return false;
    }
}

void int_solver::display_row_info(std::ostream & out, unsigned row_index) const  {
    auto & rslv = m_lar_solver->m_mpq_lar_core_solver.m_r_solver;
    for (const auto &c: rslv.m_A.m_rows[row_index]) {
        if (numeric_traits<mpq>::is_pos(c.coeff()))
            out << "+";
        out << c.coeff() << rslv.column_name(c.var()) << " ";
    }

    for (const auto& c: rslv.m_A.m_rows[row_index]) {
        rslv.print_column_bound_info(c.var(), out);
    }
    rslv.print_column_bound_info(rslv.m_basis[row_index], out);
}

unsigned int_solver::random() {
    return m_lar_solver->get_core_solver().settings().random_next();
}


// at this point the 
bool int_solver::shift_var(unsigned j, unsigned range) {
    if (is_fixed(j) || is_base(j))
        return false;
       
    bool inf_l, inf_u;
    impq l, u;
    mpq m;
    get_freedom_interval_for_column(j, inf_l, l, inf_u, u, m);
    const impq & x = get_value(j);
    // x, the value of j column, might be shifted on a multiple of m
    if (inf_l && inf_u) {
        impq new_val = m * impq(random() % (range + 1)) + x;
        set_value_for_nbasic_column_ignore_old_values(j, new_val);
        return true;
    }
    if (column_is_int(j)) {
        if (!inf_l) {
            l = impq(ceil(l));
        }
        if (!inf_u) {
            u = impq(floor(u));
        }
    }
    if (!inf_l && !inf_u && l >= u)
        return false;

    if (inf_u) {
        SASSERT(!inf_l);
        impq new_val = x + m * impq(random() % (range + 1));
        set_value_for_nbasic_column_ignore_old_values(j, new_val);
        return true;
    }

    if (inf_l) {
        SASSERT(!inf_u);
        impq new_val = x - m * impq(random() % (range + 1));
        set_value_for_nbasic_column_ignore_old_values(j, new_val);
        return true;
    }

    SASSERT(!inf_l && !inf_u);
    mpq r = floor((u.x - l.x) / m);
    if (r < mpq(range)) range = static_cast<unsigned>(r.get_uint64());
    // the interval contains at least range multiples of m.
    // the number of multiples to the left of the value of j is floor((get_value(j) - l.x)/m)
    // shift either left or right of the current value by available multiples.
    impq shift = impq(random() % (range + 1)) - impq(floor(x.x - l.x) / m);
    impq new_val = x + m * shift;
    SASSERT(l <= new_val && new_val <= u);
    set_value_for_nbasic_column_ignore_old_values(j, new_val);    return true;
}

bool int_solver::non_basic_columns_are_at_bounds() const {
    auto & lcs = m_lar_solver->m_mpq_lar_core_solver;
    for (unsigned j :lcs.m_r_nbasis) {
        auto & val = lcs.m_r_x[j];
        switch (lcs.m_column_types()[j]) {
        case column_type::boxed:
            if (val != lcs.m_r_lower_bounds()[j] && val != lcs.m_r_upper_bounds()[j])
                return false;
            break;
        case column_type::lower_bound:
            if (val != lcs.m_r_lower_bounds()[j])
                return false;
            break;
        case column_type::upper_bound:
            if (val != lcs.m_r_upper_bounds()[j])
                return false;
            break;
        default:
            if (column_is_int(j) && !val.is_int()) {
                return false;
            }
        }
    }
    return true;
}

const impq& int_solver::lower_bound(unsigned j) const {
    return m_lar_solver->column_lower_bound(j);
}

lia_move int_solver::create_branch_on_column(int j) {
    TRACE("check_main_int", tout << "branching" << std::endl;);
    lp_assert(m_t.is_empty());
    lp_assert(j != -1);
    m_t.add_monomial(mpq(1), m_lar_solver->adjust_column_index_to_term_index(j));
    if (is_free(j)) {
        m_upper = random() % 2;
        m_k = mpq(0);
    } else {
        m_upper = left_branch_is_more_narrow_than_right(j);
        m_k = m_upper? floor(get_value(j)) : ceil(get_value(j));        
    }

    TRACE("int_solver",
          display_column(tout << "branching v" << j << " = " << get_value(j) << "\n", j);
          tout << "k = " << m_k << std::endl;);
    return lia_move::branch;

}

bool int_solver::left_branch_is_more_narrow_than_right(unsigned j) {
    switch (m_lar_solver->m_mpq_lar_core_solver.m_r_solver.m_column_types[j] ) {
    case column_type::fixed:
        return false;
    case column_type::boxed:
        {
            auto k = floor(get_value(j));
            return k - lower_bound(j).x < upper_bound(j).x - (k + mpq(1));
        }
    case column_type::lower_bound: 
        return true;
    case column_type::upper_bound:
        return false;
    default:
        return false;
    }       
}
    
const impq& int_solver::upper_bound(unsigned j) const {
    return m_lar_solver->column_upper_bound(j);
}

bool int_solver::is_term(unsigned j) const {
    return m_lar_solver->column_corresponds_to_term(j);
}

unsigned int_solver::column_count() const  { return m_lar_solver->column_count(); }

}
