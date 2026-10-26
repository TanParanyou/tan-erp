//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_review_approval_thresholds_response.g.dart';

/// EstimateReviewApprovalThresholdsResponse
///
/// Properties:
/// * [managerAmountLimit] 
/// * [financialAmountLimit] 
/// * [directorAmountLimit] 
/// * [checkerMinimumMarginRate] 
/// * [directorMinimumMarginRate] 
/// * [discountLimitRate] 
@BuiltValue()
abstract class EstimateReviewApprovalThresholdsResponse implements Built<EstimateReviewApprovalThresholdsResponse, EstimateReviewApprovalThresholdsResponseBuilder> {
  @BuiltValueField(wireName: r'managerAmountLimit')
  double? get managerAmountLimit;

  @BuiltValueField(wireName: r'financialAmountLimit')
  double? get financialAmountLimit;

  @BuiltValueField(wireName: r'directorAmountLimit')
  double? get directorAmountLimit;

  @BuiltValueField(wireName: r'checkerMinimumMarginRate')
  double? get checkerMinimumMarginRate;

  @BuiltValueField(wireName: r'directorMinimumMarginRate')
  double? get directorMinimumMarginRate;

  @BuiltValueField(wireName: r'discountLimitRate')
  double? get discountLimitRate;

  EstimateReviewApprovalThresholdsResponse._();

  factory EstimateReviewApprovalThresholdsResponse([void updates(EstimateReviewApprovalThresholdsResponseBuilder b)]) = _$EstimateReviewApprovalThresholdsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateReviewApprovalThresholdsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateReviewApprovalThresholdsResponse> get serializer => _$EstimateReviewApprovalThresholdsResponseSerializer();
}

class _$EstimateReviewApprovalThresholdsResponseSerializer implements PrimitiveSerializer<EstimateReviewApprovalThresholdsResponse> {
  @override
  final Iterable<Type> types = const [EstimateReviewApprovalThresholdsResponse, _$EstimateReviewApprovalThresholdsResponse];

  @override
  final String wireName = r'EstimateReviewApprovalThresholdsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateReviewApprovalThresholdsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.managerAmountLimit != null) {
      yield r'managerAmountLimit';
      yield serializers.serialize(
        object.managerAmountLimit,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.financialAmountLimit != null) {
      yield r'financialAmountLimit';
      yield serializers.serialize(
        object.financialAmountLimit,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.directorAmountLimit != null) {
      yield r'directorAmountLimit';
      yield serializers.serialize(
        object.directorAmountLimit,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.checkerMinimumMarginRate != null) {
      yield r'checkerMinimumMarginRate';
      yield serializers.serialize(
        object.checkerMinimumMarginRate,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.directorMinimumMarginRate != null) {
      yield r'directorMinimumMarginRate';
      yield serializers.serialize(
        object.directorMinimumMarginRate,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.discountLimitRate != null) {
      yield r'discountLimitRate';
      yield serializers.serialize(
        object.discountLimitRate,
        specifiedType: const FullType.nullable(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateReviewApprovalThresholdsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateReviewApprovalThresholdsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'managerAmountLimit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.managerAmountLimit = valueDes;
          break;
        case r'financialAmountLimit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.financialAmountLimit = valueDes;
          break;
        case r'directorAmountLimit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.directorAmountLimit = valueDes;
          break;
        case r'checkerMinimumMarginRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.checkerMinimumMarginRate = valueDes;
          break;
        case r'directorMinimumMarginRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.directorMinimumMarginRate = valueDes;
          break;
        case r'discountLimitRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.discountLimitRate = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateReviewApprovalThresholdsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateReviewApprovalThresholdsResponseBuilder();
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


