//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/finance_project_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_billing_summary_response.g.dart';

/// ProjectBillingSummaryResponse
///
/// Properties:
/// * [project] 
/// * [contractAmount] 
/// * [billed] 
/// * [paid] 
/// * [outstanding] 
/// * [unbilled] 
@BuiltValue()
abstract class ProjectBillingSummaryResponse implements Built<ProjectBillingSummaryResponse, ProjectBillingSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'project')
  FinanceProjectRefResponse? get project;

  @BuiltValueField(wireName: r'contractAmount')
  double? get contractAmount;

  @BuiltValueField(wireName: r'billed')
  double? get billed;

  @BuiltValueField(wireName: r'paid')
  double? get paid;

  @BuiltValueField(wireName: r'outstanding')
  double? get outstanding;

  @BuiltValueField(wireName: r'unbilled')
  double? get unbilled;

  ProjectBillingSummaryResponse._();

  factory ProjectBillingSummaryResponse([void updates(ProjectBillingSummaryResponseBuilder b)]) = _$ProjectBillingSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectBillingSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectBillingSummaryResponse> get serializer => _$ProjectBillingSummaryResponseSerializer();
}

class _$ProjectBillingSummaryResponseSerializer implements PrimitiveSerializer<ProjectBillingSummaryResponse> {
  @override
  final Iterable<Type> types = const [ProjectBillingSummaryResponse, _$ProjectBillingSummaryResponse];

  @override
  final String wireName = r'ProjectBillingSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectBillingSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.project != null) {
      yield r'project';
      yield serializers.serialize(
        object.project,
        specifiedType: const FullType(FinanceProjectRefResponse),
      );
    }
    if (object.contractAmount != null) {
      yield r'contractAmount';
      yield serializers.serialize(
        object.contractAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.billed != null) {
      yield r'billed';
      yield serializers.serialize(
        object.billed,
        specifiedType: const FullType(double),
      );
    }
    if (object.paid != null) {
      yield r'paid';
      yield serializers.serialize(
        object.paid,
        specifiedType: const FullType(double),
      );
    }
    if (object.outstanding != null) {
      yield r'outstanding';
      yield serializers.serialize(
        object.outstanding,
        specifiedType: const FullType(double),
      );
    }
    if (object.unbilled != null) {
      yield r'unbilled';
      yield serializers.serialize(
        object.unbilled,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectBillingSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectBillingSummaryResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'project':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(FinanceProjectRefResponse),
          ) as FinanceProjectRefResponse?;
          if (valueDes == null) continue;
          result.project.replace(valueDes);
          break;
        case r'contractAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.contractAmount = valueDes;
          break;
        case r'billed':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.billed = valueDes;
          break;
        case r'paid':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.paid = valueDes;
          break;
        case r'outstanding':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.outstanding = valueDes;
          break;
        case r'unbilled':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unbilled = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectBillingSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectBillingSummaryResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


