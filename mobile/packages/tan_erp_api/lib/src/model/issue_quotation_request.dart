//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'issue_quotation_request.g.dart';

/// IssueQuotationRequest
///
/// Properties:
/// * [expectedEstimateVersion] 
/// * [expectedOpportunityVersion] 
@BuiltValue()
abstract class IssueQuotationRequest implements Built<IssueQuotationRequest, IssueQuotationRequestBuilder> {
  @BuiltValueField(wireName: r'expectedEstimateVersion')
  String? get expectedEstimateVersion;

  @BuiltValueField(wireName: r'expectedOpportunityVersion')
  String? get expectedOpportunityVersion;

  IssueQuotationRequest._();

  factory IssueQuotationRequest([void updates(IssueQuotationRequestBuilder b)]) = _$IssueQuotationRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(IssueQuotationRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<IssueQuotationRequest> get serializer => _$IssueQuotationRequestSerializer();
}

class _$IssueQuotationRequestSerializer implements PrimitiveSerializer<IssueQuotationRequest> {
  @override
  final Iterable<Type> types = const [IssueQuotationRequest, _$IssueQuotationRequest];

  @override
  final String wireName = r'IssueQuotationRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    IssueQuotationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.expectedEstimateVersion != null) {
      yield r'expectedEstimateVersion';
      yield serializers.serialize(
        object.expectedEstimateVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.expectedOpportunityVersion != null) {
      yield r'expectedOpportunityVersion';
      yield serializers.serialize(
        object.expectedOpportunityVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    IssueQuotationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required IssueQuotationRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'expectedEstimateVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedEstimateVersion = valueDes;
          break;
        case r'expectedOpportunityVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedOpportunityVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  IssueQuotationRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = IssueQuotationRequestBuilder();
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


